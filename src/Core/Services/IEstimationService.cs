
using Core.Models;

public interface IEstimationService
{
    public Estimation HandleUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag>? tags);
    public IEnumerable<Estimation> GetAllUserEstimations(string userGuid);
}
