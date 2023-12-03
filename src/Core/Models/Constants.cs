public class Constants
{
    public static string NPZ_FILENAME = "estimation.npz";
    public static string JOINTS_FILENAME = "joints.json";
    public static string PREVIEW_FILENAME = "preview.mp4";
    public static string MOTIONCAPTURE_FILENAME = "motioncapture.bvh";
    public static string ANIMATION_FILENAME = "animation.fbx";
    public static int MAXPROCESSING = 3;


    public static string GetFilename(AttachmentType attachmentType)
    {
        var attachmentName =
        attachmentType == AttachmentType.Joints ? Constants.JOINTS_FILENAME
        : attachmentType == AttachmentType.Preview ? Constants.PREVIEW_FILENAME
        : attachmentType == AttachmentType.Bvh ? Constants.MOTIONCAPTURE_FILENAME
        : attachmentType == AttachmentType.Fbx ? Constants.ANIMATION_FILENAME
        : attachmentType == AttachmentType.TFbx ? "T_" + Constants.ANIMATION_FILENAME
        : attachmentType == AttachmentType.TBvh ? "T_" + Constants.MOTIONCAPTURE_FILENAME
        : Constants.NPZ_FILENAME;

        return attachmentName;
    }
}
