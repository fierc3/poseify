import bpy
import sys


def parse_args():
    return sys.argv

def main(args):
    print(args)
    print("BVH Input: " + args[5])
    generateFbx(args[5])

def markDeformBones():
    armature = bpy.context.object
    bpy.ops.object.mode_set(mode='EDIT')
    for bone in armature.data.edit_bones:
        bone.use_deform = True

    bpy.ops.object.mode_set(mode='OBJECT')

def fillBones(originBone, targetBone, boneName):
    if bpy.context.object.type == 'ARMATURE':
        armature = bpy.context.object

        # Switch to Edit Mode
        bpy.ops.object.mode_set(mode='EDIT')

        # Deselect all
        bpy.ops.armature.select_all(action='DESELECT')

        # Access and select the heads of specific bones
        bones = armature.data.edit_bones
        if originBone in bones and targetBone in bones:
            bones[originBone].select_tail = True
            bones[targetBone].select_head = True

        bpy.ops.armature.fill()
        new_bone = bpy.context.active_bone
        if new_bone:
            new_bone.name = boneName
            new_bone.parent = bones[originBone]
            bones[targetBone].parent = new_bone

    else:
        print("No Armature selected")

def rotateOrigin():
    bpy.ops.object.mode_set(mode='OBJECT')
    bpy.context.object.rotation_euler[0] = 1.5708
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=False)


def improveArmature():
    rotateOrigin()
    fillBones("Spine", "RightShoulder", "RightCollar")
    fillBones("Spine", "LeftShoulder", "LeftCollar")
    fillBones("Hip", "Spine", "LowerBack")
    markDeformBones()

def generateFbx(bvh_file_location):
    bpy.ops.import_anim.bvh(filepath=bvh_file_location, filter_glob="*.bvh", global_scale=1, frame_start=1, use_fps_scale=False, use_cyclic=False, rotate_mode='NATIVE', axis_forward='-Z', axis_up='Y')
    improveArmature()
    bpy.ops.export_scene.fbx(filepath=bvh_file_location+".fbx", axis_forward='-Z', axis_up='Y', use_selection=True, use_armature_deform_only=True, add_leaf_bones=True)

if __name__ == '__main__':
    print("########### Starting FBX generation")
    args = parse_args()
    main(args)
