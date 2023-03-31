using Backend.Controllers;
using Raven.Client.Documents;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class EstimationHandler : IEstimationHandler
{
    private readonly ILogger<EstimationHandler> _logger;
    private readonly IDocumentStore _store;

    public EstimationHandler(ILogger<EstimationHandler> logger)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
    }

    public Estimation HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag> tags)
    {
        _ = runEstimation(userGuid, directory, fileName, fileExtension);
        string estimationPath = $"{directory}/{userGuid}/{fileName}.npz";
        Estimation estimation = storeEstimationResultToDb(userGuid, estimationPath, fileName, tags, displayName);
        //todo delete uploaded and resulting file from filesystem
        return estimation;
    }

    //todo make this async
    private bool runEstimation(string userGuid, string directory, string fileName, string fileExtension) {
        //run python estimation script
        //$"python .\estimate_pose.py --dir {directory} --guid {fileName} --file-ext {fileExtension} --scale-fps true
        //on successful completion return true
        return true;
    }

    //create a new estimation entry and attach a file to it
    private Estimation storeEstimationResultToDb(string userGuid, string path, string file_name, IEnumerable<Tag>? tags, string display_name) {
        string guid = Guid.NewGuid().ToString();
        Estimation? estimation = null;
        using (var session = _store.OpenSession())
        using (var file = File.Open(path, FileMode.Open))
        {
            estimation = new()
            {
                InternalGuid = guid,
                DisplayName = display_name,
                Tags = tags.Select(x => x.InternalGuid),
                UploadingProfile = userGuid,
                UploadDate = DateTime.Now
            };
            session.Store(estimation, guid);
            session.Advanced.Attachments.Store(guid, file_name, file);
            session.SaveChanges();
            estimation = session.Query<Estimation>().Where(x => x.InternalGuid == guid).FirstOrDefault();
        }
        return estimation;
    }
}

