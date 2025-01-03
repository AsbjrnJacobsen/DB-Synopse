using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace InventoryService.Messaging
{
    public class DeadLetterQueueConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public DeadLetterQueueConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            
            
            // Declare the DLQ
            _channel.QueueDeclare(queue: "dlx_order_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[DLQ Consumer] Dead-lettered message: {message}");
            };

            _channel.BasicConsume(queue: "dlx_order_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}