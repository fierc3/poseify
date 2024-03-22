using Core.Models;
using Core.Services.Estimations;
using Raven.Client.Documents;

namespace Core.Services.Queues
{
    public interface IQueueService
    {
        public void SendVideo2Pose(Guid estimationId, string type);
    }
}
