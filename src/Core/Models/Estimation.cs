using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class Estimation
{
    public string Guid { get; set; } = "";
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public string UploadingProfile { get; set; } = null;
    public DateTime UploadDate { get; set; } = DateTime.Now;

}