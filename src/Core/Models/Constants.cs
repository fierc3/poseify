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
            switch (attachmentType)
            {
                case AttachmentType.Joints:
                    return JOINTS_FILENAME;
                case AttachmentType.Preview:
                    return PREVIEW_FILENAME;
                case AttachmentType.Bvh:
                    return MOTIONCAPTURE_FILENAME;
                case AttachmentType.Fbx:
                    return ANIMATION_FILENAME;
                case AttachmentType.TFbx:
                    return "T_" + ANIMATION_FILENAME;
                case AttachmentType.TBvh:
                    return "T_" + MOTIONCAPTURE_FILENAME;
                default:
                    return NPZ_FILENAME;
            }
        }
    }
}