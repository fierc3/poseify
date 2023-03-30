
using Backend.Controllers;
using Raven.Client.Documents;
using System;

public class EstimationHandler
{
    private readonly ILogger<EstimationHandler> _logger;
    private readonly IDocumentStore _store;

    public EstimationHandler(ILogger<EstimationHandler> logger)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
    }


    public string HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag> tags)
    {
        _ = runEstimation(userGuid, directory, fileName, fileExtension);
        string estimationPath = $"{directory}/{userGuid}/{fileName}.npz";
        string resultGuid = storeEstimationResultToDb(userGuid, estimationPath, fileName, tags, displayName);
        //delete file from filesystem
        return resultGuid;
    }

    //todo async
    private bool runEstimation(string userGuid, string directory, string fileName, string fileExtension) {
        //run python estimation script
        //$"python .\estimate_pose.py --dir {directory} --guid {fileName} --file-ext {fileExtension} --scale-fps true
        //on successful completion return true
        return true;
    }

    private string storeEstimationResultToDb(string userGuid, string path, string file_name, IEnumerable<Tag>? tags, string display_name) {
        string guid = Guid.NewGuid().ToString();
        // storing a new estimation ravenDB, file attached to the entry
        using (var session = _store.OpenSession())
        using (var file = File.Open(path, FileMode.Open))
        {
            Estimation estimation = new()
            {
                Guid = guid,
                DisplayName = display_name,
                Tags = tags.Select(x => x.InternalGuid),
                UploadingProfile = userGuid,
                UploadDate = DateTime.Now
            };
            session.Store(estimation, guid);
            session.Advanced.Attachments.Store(guid, file_name, file);
            session.SaveChanges();
        }
        return guid;
    }
}

