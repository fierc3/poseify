using Core.Models;
using Core.Services.Fbx;
using Core.Services.Queues;
using Raven.Client.Documents;
using System.Diagnostics;

namespace Core.Services.Estimations
{
    public class EstimationService : IEstimationService
    {
        private readonly ILogger<EstimationService> _logger;
        private readonly IDocumentStore _store;
        private readonly IConfiguration _configuration;
        private readonly IQueueService _queueService;
        private readonly IFbxService _fbxService;

        public static Dictionary<string, Process> RunningProcesses = new Dictionary<string, Process>();

        public EstimationService(ILogger<EstimationService> logger, IConfiguration configuration, IQueueService queueService, IFbxService fbxService)
        {
            _logger = logger;
            _store = DocumentStoreHolder.Store;
            _configuration = configuration;
            _queueService = queueService;
            _fbxService = fbxService;
        }

        public IEnumerable<Estimation> GetAllUserEstimations(string userGuid)
        {
            List<Estimation> estimations = new List<Estimation>();
            using (var session = _store.OpenSession())
            {
                estimations = session.Query<Estimation>().Where(x => x.UploadingProfile == userGuid).OrderByDescending(x => x.ModifiedDate).ToList();
            }
            return estimations;
        }

        public Estimation? RegisterEstimation(string displayName, IEnumerable<string> tags, string userGuid)
        {
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
            }
            return estimation;
        }

        public void UpdateEstimation(Estimation editedEstimation)
        {
            using var session = _store.OpenSession();
            session.Store(editedEstimation, editedEstimation.InternalGuid);
            session.SaveChanges();
        }

        private Estimation? SetEstimationToFailed(string estimationid, string errorMessage)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid == estimationid).FirstOrDefault();
                if (estimation == null) return null;
                estimation.State = EstimationState.Failed;
                estimation.StateText = errorMessage;
                UpdateEstimation(estimation);
                return estimation;
            }
        }

        public void HandleUploadedFile(string userGuid, string directory, string fileName, string fileExtension, string displayName, IEnumerable<string> tags)
        {
            // ensure file extension is without dot
            fileExtension = fileExtension.Replace(".", "");

            var estimation = RegisterEstimation(displayName, tags, userGuid);

            if (estimation == null)
            {
                throw new Exception("Estimation could not be registered");
            }

            using (var session = _store.OpenSession())
            {
                var needsToBeQueued = session.Query<Estimation>().Where(x => x.State == EstimationState.Processing).Count() > Constants.MAXPROCESSING;
                if (needsToBeQueued)
                {
                    estimation.State = EstimationState.Queued;
                    estimation.StateText = "Currently queued, processing continues when gpu is available";
                    UpdateEstimation(estimation);
                    _ = _queueService.AddToQueueAsync(estimation, this, _store, userGuid, directory, fileName, fileExtension);
                    return;
                }
            }

            RunFile(userGuid, directory, fileName, fileExtension, estimation);
        }

        public void RunFile(string userGuid, string directory, string fileName, string fileExtension, Estimation estimation)
        {
            // Load config
            var estimatorConfigs = _configuration.GetSection("Estimators").Get<List<EstimatorConfig>>();
            // We always use VideoPose3D for the moment
            string targetEstimatorName = "VideoPose3d";
            EstimatorConfig? estimatorConfig = estimatorConfigs?.Find(estimator => estimator.Id == targetEstimatorName);

            if (estimatorConfig == null 
                || estimatorConfig.EstimationScriptLocation == null 
                || estimatorConfig.EstimatePython == null
                || estimatorConfig.FbxScriptLocation == null)
            {
                throw new Exception("No valid config found for the requested Estimator");
            }

            // Config based files and scripts
            string estimationScriptLocation = estimatorConfig.EstimationScriptLocation;
            string estimatePython = estimatorConfig.EstimatePython;
            string fbxScript = estimatorConfig.FbxScriptLocation;

            // Relative files and scripts
            string fileLocation = $"{directory}\\{userGuid}\\{fileName}";
            string estimationPath = $"{fileLocation}.{fileExtension}.npz";
            string npyPath = $"{fileLocation}_result.npy";
            string inputPath = $"{fileLocation}.{fileExtension}";
            string previewPath = $"{fileLocation}_result.mp4";
            string jointPath = $"{fileLocation}_result.json";
            string bvhPath = $"{fileLocation}_motioncapture.bvh";
            string fbxPath = $"{bvhPath}.fbx";

            try
            {
                RunEstimation(userGuid, directory, fileName, fileExtension, estimation.InternalGuid);
                _fbxService.CreateFbxFileFromBvh(bvhPath);
                File.Delete($"{inputPath}");
            }
            catch (Exception ex)
            {
                SetEstimationToFailed(estimation.InternalGuid, "Error during run of estimation: " + ex.Message);
                File.Delete($"{inputPath}");
                return;
            }

            StoreEstimationResultToDb(estimation, estimationPath, jointPath, previewPath, fileName, bvhPath, fbxPath);
            File.Delete($"{jointPath}");
            File.Delete($"{previewPath}");
            File.Delete($"{estimationPath}");
            File.Delete($"{npyPath}");
            File.Delete($"{bvhPath}");
            File.Delete($"{fbxPath}");

            return;
        }

        public Stream? GetEstimationAttachment(string estimationid, AttachmentType attachmentType, string userGuid)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid == estimationid && x.UploadingProfile == userGuid).FirstOrDefault();
                if (estimation == null)
                {
                    throw new Exception("Estimation could not be found");
                }

                var result = session.Advanced.Attachments.Get(estimation, Constants.GetFilename(attachmentType));
                return result.Stream;
            }
        }

        public void DeleteEstimation(string estimationid, string userGuid)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid == estimationid && x.UploadingProfile == userGuid).FirstOrDefault();
                if (estimation == null)
                {
                    throw new Exception("Estimation could not be found");
                }

                var processes = RunningProcesses.Where(x => x.Key.Equals(estimationid)).ToList();

                if (processes.Any())
                {
                    foreach (var process in processes)
                    {
                        if (process.Value != null) process.Value.Kill(true);
                    }
                }

                session.Delete(estimation);
                session.SaveChanges();
            }
        }

        private void RunEstimation(string userGuid, string directory, string fileName, string fileExtension, string estimationGuid)
        {
            int totalFrames = 0;
            string? estimationScriptLocation = _configuration["EstimationScriptLocation"];
            string? estimatePython = _configuration["EstimatePython"];
            Exception? exception = null;
            Process estimationProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = string.IsNullOrEmpty(estimatePython) ? "python" : estimatePython,
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
                        totalFrames = int.TryParse(ev.Data.Split(':').Last(), out totalFrames) ? totalFrames : 666;
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

            try
            {
                estimationProcess.Start();
                estimationProcess.BeginOutputReadLine();
                estimationProcess.BeginErrorReadLine();
                RunningProcesses.Add(estimationGuid, estimationProcess);
                estimationProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                SetEstimationToFailed(estimationGuid, $"Error during execution of python process of estimation {estimationGuid}: " + ex.Message);
            }
            finally
            {
                RunningProcesses.Remove(estimationGuid);
            }

            if (exception != null)
            {
                throw exception;
            }

            return;
        }

        //create a new estimation entry and attach a file to it
        private void StoreEstimationResultToDb(Estimation estimation, string estimationPath, string jointPath, string previewPath, string file_name, string bvhPath, string fbxPath)
        {
            if (estimation == null)
            {
                throw new Exception("Estimation is missing");
            }

            string guid = estimation.InternalGuid;
            using (var session = _store.OpenSession())
            using (var estimationFile = File.Open(estimationPath, FileMode.Open))
            using (var jointFile = File.Open(jointPath, FileMode.Open))
            using (var bvhFile = File.Open(bvhPath, FileMode.Open))
            using (var fbxFile = File.Open(fbxPath, FileMode.Open))
            using (var previewFile = File.Open(previewPath, FileMode.Open))
            {
                estimation.State = EstimationState.Success;
                estimation.StateText = "Successfull estimation";
                session.Store(estimation, guid);
                session.Advanced.Attachments.Store(guid, Constants.NPZ_FILENAME, estimationFile);
                session.Advanced.Attachments.Store(guid, Constants.PREVIEW_FILENAME, previewFile);
                session.Advanced.Attachments.Store(guid, Constants.JOINTS_FILENAME, jointFile);
                session.Advanced.Attachments.Store(guid, Constants.MOTIONCAPTURE_FILENAME, bvhFile);
                session.Advanced.Attachments.Store(guid, Constants.ANIMATION_FILENAME, fbxFile);
                session.SaveChanges();
            }
        }
    }
}