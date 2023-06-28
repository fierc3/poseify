from bvh_skeleton import h36m_skeleton
import numpy as np
import ast

def read_bvh_data(npy, out):
    # Read the content of the file
    with open(npy, "r") as file:
        file_content = file.read()

    # Parse the string into a nested list
    nested_list = ast.literal_eval(file_content)

    # Convert the nested list to a NumPy array
    data = np.array(nested_list)
    output = data

    # Create an instance of the H36mSkeleton class
    skel = h36m_skeleton.H36mSkeleton()

    # Rotate 180 degrees around the z-axis
    for frame in output:
        for pos in frame:
            if len(pos) < 2:
                continue
            pos[0] = -pos[0]
            pos[1] = -pos[1]

    # Set initial position to have hip centered on the x-y plane and the lowest foot on the ground
    first = output[0]
    h = first[skel.keypoint2index['Hip']]
    l = first[skel.keypoint2index['LeftAnkle']]
    r = first[skel.keypoint2index['RightAnkle']]
    d = np.array([-h[0], -min(l[1], r[1]), -h[2]])
    for frame in output:
        for pos in frame:
            pos += d

    # Rotate 90 degrees around the x-axis (for Maya)
    for frame in output:
        for pos in frame:
            y = pos[1]
            pos[1] = pos[2]
            pos[2] = -y

    # Convert the poses to BVH format and write to the output file
    skel.poses2bvh(output, output_file=out)
