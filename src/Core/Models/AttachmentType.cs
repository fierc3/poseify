using System.Text.Json.Serialization;

namespace Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AttachmentType
    {
        Preview,
        Joints,
        Npz,
        Bvh,
        TBvh,
        Fbx,
        TFbx
    }
}