using Backend.Controllers;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

public class EstimationHandler : IEstimationHandler
{
    private readonly ILogger<EstimationHandler> _logger;
    private readonly IDocumentStore _store;
    private readonly IConfiguration _configuration;

    public EstimationHandler(ILogger<EstimationHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
        _configuration = configuration;
    }

    public Estimation HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag> tags) {
        _ = runEstimation(userGuid, directory, fileName, fileExtension);
        string estimationPath = $"{directory}/{userGuid}/{fileName}.npz";
        Estimation estimation = storeEstimationResultToDb(userGuid, estimationPath, fileName, tags, displayName);
        //todo delete uploaded and resulting file from filesystem
        return estimation;
    }

    //todo make this async
    private bool runEstimation(string userGuid, string directory, string fileName, string fileExtension) {
        string? estimationScriptLocation = _configuration["EstimationScriptLocation"];
        string? overridePython = _configuration["OverridePythonVersion"];
        Process estimationProcess = new Process{
            StartInfo = new ProcessStartInfo
            {
                FileName = overridePython ?? "python",
                Arguments = $"-u {estimationScriptLocation} --dir {directory}/{userGuid} --guid {fileName} --file-ext {fileExtension}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };
        estimationProcess.ErrorDataReceived += (se, ev) => {
            _logger.Log(LogLevel.Error, ev.Data);
            //todo handle errors
        };
        estimationProcess.OutputDataReceived += (se, ev) => {
            _logger.Log(LogLevel.Information, ev.Data);
            //todo update progress
        };

        estimationProcess.Start();
        estimationProcess.BeginErrorReadLine();
        estimationProcess.BeginOutputReadLine();
        estimationProcess.WaitForExit();
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

