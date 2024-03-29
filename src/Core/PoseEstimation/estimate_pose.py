from ffmpeg import FFmpeg,Progress
import subprocess
import argparse
import sys
import os
import output2bvh

# Copyright Shadow Wizard Money Gang & Mind Goblin

def parse_args():
    parser = argparse.ArgumentParser(description='Pose estimation')
    parser.add_argument(
        '--dir',
        dest='file_dir',
        help='upload file directory',
        default='C:\\PoseifyUploads\\',
        type=str
    )
    parser.add_argument(
        '--user-id',
        dest='user_id',
        help='user_id',
        default=False,
        type=str
    )
    parser.add_argument(
        '--guid',
        dest='guid',
        help='guid',
        default=False,
        type=str
    )
    parser.add_argument(
        '--file-ext',
        dest='file_extension',
        help='file extension (default: mp4)',
        default='mp4',
        type=str
    )
    parser.add_argument(
        '--new-file-ext',
        dest='new_file_extension',
        help='new file extension (converts from file-ext to new-file-ext)',
        default=False,
        type=str
    )
    parser.add_argument(
        '--scale-fps',
        dest='scale_fps',
        help='if True scales video to 50FPS for best result',
        default=False,
        type=bool
    )
    if len(sys.argv) == 1:
        parser.print_help()
        sys.exit(1)
    return parser.parse_args()

def main(args):
    estimate_pose_for_video(args.file_dir, args.user_id, args.guid, args.file_extension, args.new_file_extension, args.scale_fps)

def estimate_pose_for_video(file_dir, user_id, guid, file_extension, new_file_extension=False, scale_fps=False):
    file_dir.replace('/', '\\')
    if not file_dir.endswith('\\'):
        file_dir += '\\'
    directory = f"{file_dir}{user_id or ''}"
    file_user_guid = f"{file_dir}{user_id or ''}\\{guid}"
    input_video_location = f"{file_user_guid}.{file_extension}"
    estimation_result_location = f"{file_user_guid}_result"

    if new_file_extension or scale_fps:
        print('###### Using FFmpeg conversion')
        input_video_location = ffmpeg_conversion(input_video_location, file_user_guid, new_file_extension, scale_fps)
        file_extension = new_file_extension or file_extension
    
    wdir = os.path.dirname(os.path.realpath(__file__)) + '\\vp3d'
    print(f'Wokring directory for vp3d: {wdir}')

    print(f'Total Frames: {get_frame_count(input_video_location)}')
    command = f'python {wdir}\\inference\\infer_video_d2.py --cfg COCO-Keypoints/keypoint_rcnn_R_101_FPN_3x.yaml --output-dir {directory} --image-ext {file_extension} {input_video_location}'
    print('-----------------------------')
    print('infer_video_2d.py, 2D joint position inference by Detectron2')
    print(command)
    print('-----------------------------')
    p0 = subprocess.run(command)
    if p0.returncode != 0:
        raise Exception( f'Invalid result in infer_video_2d.py: { p0.returncode }' )

    command = f'python prepare_data_2d_custom.py -i {directory} -o {guid}'
    print('-----------------------------')
    print('prepare_data_2d_custom.py, Preparing data for VideoPose3D')
    print(command)
    print('-----------------------------')
    p1 = subprocess.run(command, cwd=f'{wdir}\\data')
    if p1.returncode != 0:
        raise Exception( f'Invalid result in prepare_data_2d_custom.py: { p1.returncode }' )

    command = f'python {wdir}\\run.py -d custom -k {guid} -arc 3,3,3,3,3 -c checkpoint --evaluate pretrained_h36m_detectron_coco.bin --render --viz-subject {guid}.{file_extension} --viz-action custom --viz-camera 0 --viz-video {input_video_location} --viz-export {estimation_result_location} --viz-output {estimation_result_location}.mp4 --viz-size 6'
    print('------------------------------')
    print('run.py, Generating 3D Joint Positions with VideoPose3D')
    print(command)
    print('------------------------------')
    p2 = subprocess.run(command, cwd=f'{wdir}')
    if p2.returncode != 0:
        raise Exception( f'Invalid result in run.py: { p2.returncode }' )
    
    print('------------------------------')
    print('Generating bvh data with  output2bvh.py')
    print('------------------------------')
    output2bvh.read_bvh_data(f"{file_user_guid}_result.json", f"{file_user_guid}_motioncapture.bvh")

    print('------------------------------')
    print('Generating bvh data with frame 0 including T Pose')
    print('------------------------------')
    input_non_tpose = f"{file_user_guid}_motioncapture.bvh"
    output_tpose = f"{file_user_guid}_tpose.bvh"

    # Read the content of the original file
    with open(input_non_tpose, 'r') as file:
        content = file.read()

    # Replace the specified value
    content = content.replace("Frame Time: 0.03333333333333333", "Frame Time: 0.03333333333333333\n0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0")

    # Write the modified content to the new file
    with open(output_tpose, 'w') as file:
        file.write(content)

def ffmpeg_conversion(input_video_location, file_user_guid, new_file_extension=False, scale_fps=False):
    if not new_file_extension:
        output_video_location = input_video_location
        input_video_location += "_old"
        os.system(f'copy {output_video_location} {input_video_location}')
    else:
        output_video_location = f"{file_user_guid}.{new_file_extension}"
    
    if scale_fps:
        command = ['ffmpeg', '-hwaccel cuvid', '-hide_banner', '-loglevel', 'error','-y', '-i', f'{input_video_location}', '-filter', "minterpolate='fps=50'", '-crf', '0', f'{output_video_location}']
    else:
        command = ['ffmpeg', '-hwaccel cuvid', '-hide_banner', '-loglevel', 'error','-y', '-i', f'{input_video_location}', '-crf', '0', f'{output_video_location}']
    subprocess.run(command)
    return output_video_location

def get_frame_count(input_video_location):
    command = ['ffprobe', '-v', 'error', '-select_streams', 'v:0', '-count_packets',
               '-show_entries', 'stream=nb_read_packets ', '-of', 'csv=p=0', input_video_location]
    pipe = subprocess.Popen(command, stdout=subprocess.PIPE, bufsize=-1)
    for line in pipe.stdout:
        a = line.decode().strip()
        return a

if __name__ == '__main__':
    args = parse_args()
    main(args)
