using Core.Models;

namespace Core.Services.Estimations
{
    public interface IEstimationService
    {
        public void HandleUploadedFile(string userGuid, string directory, Guid fileId, string displayName, IEnumerable<string> tags);
        public IEnumerable<Estimation> GetAllUserEstimations(string userGuid);
        public Stream? GetEstimationAttachment(string estimationid, AttachmentType attachmentType, string userGuid);
        public void DeleteEstimation(string estimationid, string userGuid);
        public Estimation? SetEstimationToFailed(string estimationId, string errorMessage);
        public Estimation GetEstimation(string estimationId);
        public void StoreEstimationResultToDb(Estimation estimation, string bvhPath, string fbxPath, string bvhTPose, string fbxTPose);
    }
}