using System.Text.Json.Serialization;

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
