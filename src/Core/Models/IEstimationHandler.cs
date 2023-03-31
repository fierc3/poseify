
public interface IEstimationHandler
{
    public Estimation HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag>? tags);
}
