using Core.Models;
using Raven.Client.Documents;
using System;
using System.Diagnostics;
using System.IO;

namespace Core.Services
{
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

        public IEnumerable<Estimation> GetAllUserEstimations(string userGuid) 
        {
            List<Estimation> estimations = new List<Estimation>();
            using (var session = _store.OpenSession()) {
                estimations = session.Query<Estimation>().Where(x => x.UploadingProfile == userGuid).ToList();
            }
            return estimations;
        }
        
        public Estimation? RegisterEstimation(string displayName, IEnumerable<Tag>? tags, string userGuid) {
            string guid = Guid.NewGuid().ToString();
            Estimation? estimation = null;

            using (var session = _store.OpenSession())
            {
                estimation = new()
                {
                    InternalGuid = guid,
                    DisplayName = displayName,
                    Tags = tags?.Select(x => x.InternalGuid).ToList(),
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
    
        public Estimation HandleUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<Tag>? tags)
        {
            var estimation = RegisterEstimation(displayName, tags, userGuid);
            if(estimation == null)
            {
                throw new Exception("Estimation could not be registered");
            }
            try
            {
                RunEstimation(userGuid, directory, fileName, fileExtension);
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

        //todo make this async?
        private void RunEstimation(string userGuid, string directory, string fileName, string fileExtension)
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
            estimationProcess.WaitForExit();

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
                session.Advanced.Attachments.Store(guid, $"{file_name}.npz", estimationFile);
                session.Advanced.Attachments.Store(guid, $"{file_name}_result.mp4", previewFile);
                session.SaveChanges();
            }
        }
    }
}