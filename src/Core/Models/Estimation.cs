using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class Estimation
{
    public string InternalGuid { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public IEnumerable<string>? Tags { get; set; } = new List<string>();
    public string UploadingProfile { get; set; } = "";
    public DateTime UploadDate { get; set; } = DateTime.Now;
}