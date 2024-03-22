using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Core.Services.Estimations;

namespace Core.Services.Queues
{
    public class QueueListenerService : BackgroundService
    {
        private readonly IEstimationService _estimationService;
        private readonly IEstimationCleanService _estimationCleanService;
        private readonly ILogger<QueueListenerService> _logger;

        public QueueListenerService(
            IEstimationService estimationService, 
            IEstimationCleanService estimationCleanService, 
            ILogger<QueueListenerService> logger)
        {
            _estimationService = estimationService;
            _logger = logger;
            _estimationCleanService = estimationCleanService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitFbx2PublishListener(stoppingToken);
            await InitErrorListener(stoppingToken);
        }

        private async Task InitFbx2PublishListener(CancellationToken stoppingToken)
        {
            // needs to be read from configuration
            var queueName = "fbx2publish";
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                string? estimationId = ea.BasicProperties.Headers["GUID"].ToString();

                if (string.IsNullOrEmpty(estimationId))
                {
                    _logger.LogWarning("Estimation id not set for fbx2publish message");
                    return;
                }

                var estimation = _estimationService.GetEstimation(estimationId);

                if (estimation == null)
                {
                    _logger.LogWarning($"Estimation from fbx2publish is not found: {estimationId}");
                    return;
                }

                _estimationService.StoreEstimationResultToDb(estimation,"estimation", "joint", "previewPath", "filen");
                _estimationCleanService.CleanAllData(estimationId);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task InitErrorListener(CancellationToken stoppingToken)
        {
            // needs to be read from configuration
            var queueName = "error";
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                string? estimationId = ea.BasicProperties.Headers["GUID"].ToString();
                string? errorMessage = ea.BasicProperties.Headers["error"].ToString();

                if (string.IsNullOrEmpty(estimationId))
                {
                    _logger.LogWarning($"Tried to read error but id was invalid: {estimationId}");
                    return;
                }
                _estimationService.SetEstimationToFailed(estimationId, errorMessage ?? "no error message");
                _estimationCleanService.CleanAllData(estimationId);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

}
