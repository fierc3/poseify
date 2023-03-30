using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class Estimation
{
    public string Guid { get; set; } = "";
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public UserProfile UploadingProfile { get; set; } = null;
    public DateTime UploadDate { get; set; } = DateTime.Now;

}
//    session.Advanced.Attachments.Store(estimation.Guid, videoName+"_estimation_result", estimationFile);
//    session.Advanced.Attachments.Store(estimation.Guid, videoName+"_preview", previewFile);