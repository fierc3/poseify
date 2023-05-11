
using Core.Models;
using Raven.Client.Documents.Attachments;

public interface IEstimationService
{
    public Estimation HandleUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<string> tags);
    public IEnumerable<Estimation> GetAllUserEstimations(string userGuid);
    public Stream? GetEstimationAttachment(string estimationid, AttachmentType attachmentType);
    public void DeleteEstimation(string estimationid);
}
