using Core.Models;
using Raven.Client.Documents;
namespace Core.Services
{
    public class QueueService
    {
        private static Queue<Estimation> queuedEstimations = new Queue<Estimation> ();
        private static PeriodicTimer? dequeueTimer;

        public static async Task AddToQueueAsync(Estimation estimation, EstimationService service, IDocumentStore _store, string userGuid, string directory, string fileName, string  fileExtension)
        {
            queuedEstimations.Enqueue(estimation);

            if(dequeueTimer == null)
            {
                dequeueTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));
                while (await dequeueTimer.WaitForNextTickAsync())
                {
                    using (var session = _store.OpenSession())
                    {
                        var canBeProcessed = session.Query<Estimation>().Where(x => x.State == EstimationState.Processing).Count() < Constants.MAXPROCESSING;
                        if (canBeProcessed && queuedEstimations.Count() > 0)
                        {
                            var queuedEstimation = queuedEstimations.Dequeue();
                            var updatedEstimation = session.Query<Estimation>().Where(x => x.InternalGuid == queuedEstimation.InternalGuid).FirstOrDefault();
                            if (updatedEstimation == null) return;

                            updatedEstimation.State = EstimationState.Processing;
                            updatedEstimation.StateText = "Resumed from queueu, processing now";
                            service.UpdateEstimation(updatedEstimation);
                            service.RunFile(userGuid, directory, fileName, fileExtension, estimation);
                        }
                    }
                }
            }
        }
    }
}
