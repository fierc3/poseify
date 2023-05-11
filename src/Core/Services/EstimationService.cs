using Core.Models;
using Raven.Client.Documents;
using System.Diagnostics;

namespace Core.Services
{
    public class EstimationService : IEstimationService
    {
        private readonly ILogger<EstimationService> _logger;
        private readonly IDocumentStore _store;
        private readonly IConfiguration _configuration;

        public static Dictionary<string, Process> RunningProcesses = new Dictionary<string, Process>();

        public EstimationService(ILogger<EstimationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _store = DocumentStoreHolder.Store;
            _configuration = configuration;
        }

        public IEnumerable<Estimation> GetAllUserEstimations(string userGuid) 
        {
            List<Estimation> estimations = new List<Estimation>();
            using (var session = _store.OpenSession()) {
                estimations = session.Query<Estimation>().Where(x => x.UploadingProfile == userGuid).OrderByDescending(x => x.ModifiedDate).ToList();
            }
            return estimations;
        }
        
        public Estimation? RegisterEstimation(string displayName, IEnumerable<string> tags, string userGuid) {
            string guid = Guid.NewGuid().ToString();
            Estimation? estimation = null;

            using (var session = _store.OpenSession())
            {
                estimation = new Estimation()
                {
                    InternalGuid = guid,
                    DisplayName = displayName,
                    Tags = tags,
                    UploadingProfile = userGuid,
                    ModifiedDate = DateTime.Now,
                    State = EstimationState.Processing
                };
                session.Store(estimation, guid);
                session.SaveChanges();
                estimation = session.Query<Estimation>().Where(x => x.InternalGuid == guid).FirstOrDefault();
            }
            return estimation;
        }

        public void UpdateEstimation(Estimation editedEstimation)
        {
            using var session = _store.OpenSession();
            session.Store(editedEstimation, editedEstimation.InternalGuid);
            session.SaveChanges();
        }
    
        public Estimation HandleUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<string> tags)
        {
            // ensure file extension is without dot
            fileExtension = fileExtension.Replace(".", "");

            var estimation = RegisterEstimation(displayName, tags, userGuid);
            if(estimation == null)
            {
                throw new Exception("Estimation could not be registered");
            }
            try
            {
                RunEstimation(userGuid, directory, fileName, fileExtension, estimation.InternalGuid);
            }
            catch
            {
                estimation.State = EstimationState.Failed;
                UpdateEstimation(estimation);
                throw;
            }

            string fileLocation = $"{directory}\\{userGuid}\\{fileName}";
            string estimationPath = $"{fileLocation}.{fileExtension}.npz";
            string previewPath = $"{fileLocation}_result.mp4";

            StoreEstimationResultToDb(estimation, estimationPath, previewPath, fileName);
            File.Delete($"{fileLocation}.mp4");
            return estimation;
        }

        public Stream? GetEstimationAttachment(string estimationid, AttachmentType attachmentType)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid == estimationid).FirstOrDefault();
                if(estimation == null)
                {
                    throw new Exception("Estimation could not be found");
                }

                var attachmentName = attachmentType == AttachmentType.Joints ? Constants.JOINTS_FILENAME : Constants.PREVIEW_FILENAME;
                var result = session.Advanced.Attachments.Get(estimation, attachmentName);
                return result.Stream;
            }
        }

        public void DeleteEstimation(string estimationid)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid == estimationid).FirstOrDefault();
                if (estimation == null)
                {
                    throw new Exception("Estimation could not be found");
                }

                var processes = RunningProcesses.Where(x => x.Key.Equals(estimationid)).ToList();

                if(processes.Any())
                {
                    foreach(var process in processes) {
                        if (process.Value != null) process.Value.Kill(true);
                    }
                }

                session.Delete(estimation);
                session.SaveChanges();
            }
        }

        //todo make this async?
        private void RunEstimation(string userGuid, string directory, string fileName, string fileExtension, string estimationGuid)
        {
            int totalFrames = 0;
            string? estimationScriptLocation = _configuration["EstimationScriptLocation"];
            string? overridePython = _configuration["OverridePythonVersion"];
            Exception? exception = null;
            Process estimationProcess = new Process
            {
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
            estimationProcess.OutputDataReceived += (se, ev) =>
            {
                _logger.Log(LogLevel.Information, ev.Data);
                if (ev.Data != null)
                {
                    if (totalFrames == 0 && ev.Data.Contains("Total Frames:"))
                    {
                        totalFrames = int.Parse(ev.Data.Split(':').Last());
                    }
                    if (ev.Data.Contains("Frame") && ev.Data.Contains("processed") || ev.Data.Contains($"/{totalFrames}")) // before or is infer2d after or is run.py
                    {
                        // bubble progress event to frontend 
                    }
                }
            };
            estimationProcess.ErrorDataReceived += (se, ev) =>
            {
                _logger.Log(LogLevel.Error, ev.Data);
                if (ev.Data != null && ev.Data.Contains("Invalid result in"))
                {
                    exception = new Exception(ev.Data);
                }
            };

            estimationProcess.Start();
            estimationProcess.BeginOutputReadLine();
            estimationProcess.BeginErrorReadLine();
            RunningProcesses.Add(estimationGuid, estimationProcess);
            estimationProcess.WaitForExit();
            RunningProcesses.Remove(estimationGuid);

            if (exception != null)
            {
                throw exception;
            }

            return;
        }

        //create a new estimation entry and attach a file to it
        private void StoreEstimationResultToDb(Estimation estimation, string estimationPath, string previewPath, string file_name)
        {
            if(estimation == null)
            {
                throw new Exception("Estimation is missing");
            }

            string guid = estimation.InternalGuid;
            using (var session = _store.OpenSession())
            using (var estimationFile = File.Open(estimationPath, FileMode.Open))
            using (var previewFile = File.Open(previewPath, FileMode.Open))
            {
                estimation.State = EstimationState.Success;
                session.Store(estimation, guid);
                session.Advanced.Attachments.Store(guid, Constants.JOINTS_FILENAME, estimationFile);
                session.Advanced.Attachments.Store(guid, Constants.PREVIEW_FILENAME, previewFile);
                session.SaveChanges();
            }
        }
    }
}