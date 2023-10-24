import bpy
import sys


def parse_args():
    return sys.argv

def main(args):
    print(args)
    print("BVH Input: " + args[5])
    generateFbx(args[5])

def generateFbx(bvh_file_location):
    bpy.ops.import_anim.bvh(filepath=bvh_file_location, filter_glob="*.bvh", global_scale=1, frame_start=1, use_fps_scale=False, use_cyclic=False, rotate_mode='NATIVE', axis_forward='-Z', axis_up='Y')
    bpy.ops.export_scene.fbx(filepath=bvh_file_location+".fbx", axis_forward='-Z', axis_up='Y', use_selection=True)


if __name__ == '__main__':
    print("########### Starting FBX generation")
    args = parse_args()
    main(args)
