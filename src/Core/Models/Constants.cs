namespace Core.Models
{
    public class Constants
    {
        public const string NPZ_FILENAME = "estimation.npz";
        public const string JOINTS_FILENAME = "joints.json";
        public const string PREVIEW_FILENAME = "preview.mp4";
        public const string MOTIONCAPTURE_FILENAME = "motioncapture.bvh";
        public const string ANIMATION_FILENAME = "animation.fbx";
        public const int MAXPROCESSING = 3;

        protected Constants()
        {
        }

        public static string GetFilename(AttachmentType attachmentType)
        {
            var attachmentName =
            attachmentType == AttachmentType.Joints ? JOINTS_FILENAME
            : attachmentType == AttachmentType.Preview ? PREVIEW_FILENAME
            : attachmentType == AttachmentType.Bvh ? MOTIONCAPTURE_FILENAME
            : attachmentType == AttachmentType.Fbx ? ANIMATION_FILENAME
            : attachmentType == AttachmentType.TFbx ? "T_" + ANIMATION_FILENAME
            : attachmentType == AttachmentType.TBvh ? "T_" + MOTIONCAPTURE_FILENAME
            : NPZ_FILENAME;

            return attachmentName;
        }
    }
}