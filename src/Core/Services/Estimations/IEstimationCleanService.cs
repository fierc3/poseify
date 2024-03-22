using Core.Models;

namespace Core.Services.Estimations
{
    public interface IEstimationCleanService
    {
        public bool CleanAllData(string estimationId);
    }
}
