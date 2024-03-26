namespace Core.Services.Queues
{
    public interface IQueueService
    {
        public void SendVideo2Pose(Guid estimationId, string type);
    }
}
