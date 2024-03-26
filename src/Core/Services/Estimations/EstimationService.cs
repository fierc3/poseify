using Core.Db;
using Core.Models;
using Core.Services.Queues;
using Raven.Client.Documents;
using System.Data;

namespace Core.Services.Estimations
{
    public class EstimationService : IEstimationService
    {
        private readonly ILogger<EstimationService> _logger;
        private readonly IDocumentStore _store;
        private readonly IConfiguration _configuration;
        private readonly IQueueService _queueService;

        public EstimationService(ILogger<EstimationService> logger, 
            IConfiguration configuration, 
            IQueueService queueService)
        {
            _logger = logger;
            _store = DocumentStoreHolder.Store;
            _configuration = configuration;
            _queueService = queueService;
        }

        public IEnumerable<Estimation> GetAllUserEstimations(string userGuid)
        {
            List<Estimation> estimations;
            using (var session = _store.OpenSession())
            {
                estimations = session.Query<Estimation>().Where(x => x.UploadingProfile == userGuid).OrderByDescending(x => x.ModifiedDate).ToList();
            }
            return estimations;
        }

        public Estimation? RegisterEstimation(Guid estimationId, string displayName, IEnumerable<string> tags, string userGuid)
        {
            Estimation? estimation = null;

            using (var session = _store.OpenSession())
            {
                estimation = new Estimation()
                {
                    InternalGuid = estimationId,
                    DisplayName = displayName,
                    Tags = tags,
                    UploadingProfile = userGuid,
                    ModifiedDate = DateTime.Now,
                    State = EstimationState.Processing
                };
                session.Store(estimation, estimationId.ToString());
                session.SaveChanges();
            }
            return estimation;
        }

        public void UpdateEstimation(Estimation editedEstimation)
        {
            using var session = _store.OpenSession();
            session.Store(editedEstimation, editedEstimation.InternalGuid.ToString());
            session.SaveChanges();
        }

        public Estimation? SetEstimationToFailed(string estimationId, string errorMessage)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid.ToString() == estimationId).FirstOrDefault();
                if (estimation == null) return null;
                estimation.State = EstimationState.Failed;
                estimation.StateText = errorMessage;
                UpdateEstimation(estimation);
                return estimation;
            }
        }

        public void HandleUploadedFile(string userGuid, string directory, Guid fileId, string displayName, IEnumerable<string> tags)
        {
            _logger.LogDebug($"Processing file that has been uploadind {_configuration["UploadDirectory"]}/{fileId}.*");

            var estimation = RegisterEstimation(fileId, displayName, tags, userGuid);
            
            if (estimation == null)
            {
                _logger.LogWarning("Failed to register Estimation in DB");
                throw new DataException("Estimation could not be registered");
            }

            _queueService.SendVideo2Pose(estimation.InternalGuid, "VideoPose3d");
        }


        public Stream? GetEstimationAttachment(string estimationid, AttachmentType attachmentType, string userGuid)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid.ToString() == estimationid && x.UploadingProfile == userGuid).FirstOrDefault();
                if (estimation == null)
                {
                    throw new FileNotFoundException("Estimation could not be found");
                }

                var result = session.Advanced.Attachments.Get(estimation, Constants.GetFilename(attachmentType));
                return result.Stream;
            }
        }

        public void DeleteEstimation(string estimationid, string userGuid)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid.ToString() == estimationid && x.UploadingProfile == userGuid).FirstOrDefault();
                if (estimation == null)
                {
                    throw new InvalidOperationException("Estimation could not be found");
                }

                if( estimation.State != EstimationState.Success && 
                    estimation.State != EstimationState.Failed)
                {
                    throw new InvalidOperationException("Estimation not in success or failed state");
                }

                session.Delete(estimation);
                session.SaveChanges();
            }
        }

        //create a new estimation entry and attach a file to it
        public void StoreEstimationResultToDb(Estimation estimation, string bvhPath, string fbxPath, string bvhTPose, string fbxTPose)
        {
            if (estimation == null)
            {
                throw new FileNotFoundException("Estimation is missing");
            }

            string guid = estimation.InternalGuid.ToString();
            using (var session = _store.OpenSession())
            using (var bvhFile = File.Open(bvhPath, FileMode.Open))
            using (var bvhTFile = File.Open(bvhTPose, FileMode.Open))
            using (var fbxFile = File.Open(fbxPath, FileMode.Open))
            using (var fbxTFile = File.Open(fbxTPose, FileMode.Open))
            {
                estimation.State = EstimationState.Success;
                estimation.StateText = "Successfull estimation";
                session.Store(estimation, guid);
                session.Advanced.Attachments.Store(guid, Constants.MOTIONCAPTURE_FILENAME, bvhFile);
                session.Advanced.Attachments.Store(guid, "T_" + Constants.MOTIONCAPTURE_FILENAME, bvhTFile);
                session.Advanced.Attachments.Store(guid, Constants.ANIMATION_FILENAME, fbxFile);
                session.Advanced.Attachments.Store(guid, "T_" + Constants.ANIMATION_FILENAME, fbxTFile);
                session.SaveChanges();
            }
        }

        public Estimation GetEstimation(string estimationId)
        {
            using (var session = _store.OpenSession())
            {
                var estimation = session.Query<Estimation>().Where(x => x.InternalGuid.ToString() == estimationId).FirstOrDefault();
                if (estimation == null)
                {
                    throw new FileNotFoundException("Estimation could not be found");
                }

                return estimation;
            }
        }
    }
}