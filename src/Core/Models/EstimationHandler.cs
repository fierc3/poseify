
using Raven.Client.Documents;
using System.Diagnostics;
using System.IO;

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

    public Estimation HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag>? tags) {
        bool exitCode = runEstimation(userGuid, directory, fileName, fileExtension);
        if (!exitCode) {
            throw new Exception();
        }
        string fileLocation = $"{directory}\\{userGuid}\\{fileName}";
        string estimationPath = $"{fileLocation}.{fileExtension}.npz";
        string previewPath = $"{fileLocation}_result.mp4";
        Estimation estimation = storeEstimationResultToDb(userGuid, estimationPath, previewPath, fileName, tags, displayName);
        File.Delete($"{fileLocation}.mp4");
        return estimation;
    }

    //todo make this async?
    private bool runEstimation(string userGuid, string directory, string fileName, string fileExtension) {
        bool exitCode = true;
        string? estimationScriptLocation = _configuration["EstimationScriptLocation"];
        string? overridePython = _configuration["OverridePythonVersion"];
        Process estimationProcess = new Process{
            StartInfo = new ProcessStartInfo
            {
                FileName = string.IsNullOrEmpty(overridePython) ? "python" : overridePython,
                Arguments = $"-u {estimationScriptLocation} --dir {directory}  --user-id {userGuid} --guid {fileName} --file-ext {fileExtension}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true,
        };
        estimationProcess.OutputDataReceived += (se, ev) => {
            _logger.Log(LogLevel.Information, ev.Data);
            //todo update progress to frontend
        };
        estimationProcess.ErrorDataReceived += (se, ev) => {
            _logger.Log(LogLevel.Error, ev.Data);
            if (ev.Data.Contains("Invalid result in")) {
                exitCode = false;
            }
        };

        estimationProcess.Start();
        estimationProcess.BeginOutputReadLine();
        estimationProcess.BeginErrorReadLine();
        estimationProcess.WaitForExit();
        return exitCode;
    }

    //create a new estimation entry and attach a file to it
    private Estimation storeEstimationResultToDb(string userGuid, string estimationPath, string previewPath, string file_name, IEnumerable<Tag>? tags, string display_name) {
        string guid = Guid.NewGuid().ToString();
        Estimation? estimation = null;

        using (var session = _store.OpenSession())
        using (var estimationFile = File.Open(estimationPath, FileMode.Open))
        using (var previewFile = File.Open(previewPath, FileMode.Open))
        {
            estimation = new()
            {
                InternalGuid = guid,
                DisplayName = display_name,
                Tags = tags?.Select(x => x.InternalGuid).ToList(),
                UploadingProfile = userGuid,
                UploadDate = DateTime.Now
            };
            session.Store(estimation, guid);
            session.Advanced.Attachments.Store(guid, $"{file_name}.npz", estimationFile);
            session.Advanced.Attachments.Store(guid, $"{file_name}_result.mp4", previewFile);
            session.SaveChanges();
            estimation = session.Query<Estimation>().Where(x => x.InternalGuid == guid).FirstOrDefault();
        }
        return estimation;
    }
}

