using Core.Models;
using Core.Services.Estimations;
using Raven.Client.Documents;

namespace Core.Services.Queues
{
    public interface IQueueService
    {
        public Task AddToQueueAsync(Estimation estimation, EstimationService service, IDocumentStore _store, string userGuid, string directory, string fileName, string fileExtension);
    }
}
