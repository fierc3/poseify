using RabbitMQ.Client;
using System.Text;

namespace Core.Services.Queues
{
    public class QueueService : IQueueService
    {
        private readonly ILogger<QueueService> _logger;

        public QueueService(ILogger<QueueService> logger)
        {
            _logger = logger;
        }

        public void SendVideo2Pose(Guid estimationId, string type)
        {
            var queueName = "video2pose";
            // we should load this via configuration in the future
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>
            {
                { "GUID", estimationId },
                { "Type", type },
            };

                var body = Encoding.UTF8.GetBytes("");

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);
                _logger.LogDebug($"Sent {estimationId} to vide2pose ");
            }
        }
    }
}
