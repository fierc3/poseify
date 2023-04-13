
using Raven.Client.Documents;
using System.Diagnostics;
using System.IO;

public class EstimationService : IEstimationService
{
    private readonly ILogger<EstimationService> _logger;
    private readonly IDocumentStore _store;
    private readonly IConfiguration _configuration;

    public EstimationService(ILogger<EstimationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
        _configuration = configuration;
    }

    public Estimation HanldeUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag>? tags) {
        RunEstimation(userGuid, directory, fileName, fileExtension);
        string fileLocation = $"{directory}\\{userGuid}\\{fileName}";
        string estimationPath = $"{fileLocation}.{fileExtension}.npz";
        string previewPath = $"{fileLocation}_result.mp4";
        Estimation estimation = StoreEstimationResultToDb(userGuid, estimationPath, previewPath, fileName, tags, displayName);
        File.Delete($"{fileLocation}.mp4");
        return estimation;
    }

    //todo make this async?
    private void RunEstimation(string userGuid, string directory, string fileName, string fileExtension) {
        int totalFrames = 0;
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
            if (ev.Data != null) {
                if (totalFrames == 0 && ev.Data.Contains("Total Frames:")) {
                    totalFrames = int.Parse(ev.Data.Split(':').Last());
                }
                if (ev.Data.Contains("Frame") && ev.Data.Contains("processed") || ev.Data.Contains($"/{totalFrames}")) // before or is infer2d after or is run.py
                { 
                    // bubble progress event to frontend 
                }
            }
        };
        estimationProcess.ErrorDataReceived += (se, ev) => {
            _logger.Log(LogLevel.Error, ev.Data);
            if (ev.Data != null) {
                if (ev.Data.Contains("Invalid result in")) {
                    throw new Exception(ev.Data);
                }
                // todo other errors might occur but harmless, still have to handle them
                
            }
        };

        estimationProcess.Start();
        estimationProcess.BeginOutputReadLine();
        estimationProcess.BeginErrorReadLine();
        estimationProcess.WaitForExit();
        return;
    }

    //create a new estimation entry and attach a file to it
    private Estimation StoreEstimationResultToDb(string userGuid, string estimationPath, string previewPath, string file_name, IEnumerable<Tag>? tags, string display_name) {
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

