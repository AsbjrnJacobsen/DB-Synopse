using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using InventoryService.Model;
namespace InventoryService.Messaging
{
    public class InventoryMessageConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public InventoryMessageConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            
            using var connection = factory.CreateConnection();


            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Declare exchange
            _channel.ExchangeDeclare(exchange: "order_exchange", type: ExchangeType.Direct);

            // Declare queue
            _channel.QueueDeclare(queue: "order_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            // Bind Queue
            _channel.QueueBind(queue: "order_queue", exchange: "order_exchange", routingKey: "orderRK");
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Payload>(message);

                Console.WriteLine($"---------[InventoryMessageConsumer]-------- OrderID received: {order.OrderDto.OrderId}, {order}");

                try
                {
                    var isStockAvailable = CheckInventory(order.OrderDto.ProductId, order.Quantity);

                    if (!isStockAvailable)
                        throw new Exception("Insufficient stock");

                    Console.WriteLine($"---------[InventoryMessageConsumer]--------- Stock confirmed for OrderID {order.OrderDto.OrderId}, ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"---------[InventoryMessageConsumer]--------- Error: {ex.Message}");
                    MoveToDeadLetterQueue(body, ex.Message);
                }
            };

            _channel.BasicConsume(queue: "order_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private bool CheckInventory(int? productId, int quantity)
        {
            // Placeholder for MongoDB stock check
            return quantity <= 100; // Simulated stock check
        }

        private void MoveToDeadLetterQueue(byte[] body, string error)
        {

            
            _channel.QueueDeclare(queue: "dlx_order_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var properties = _channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                { "error", error }
            };

            _channel.BasicPublish(exchange: "dlx_order_queue",
                                  routingKey: "dlx_order_queue",
                                  basicProperties: properties,
                                  body: body);

            Console.WriteLine($"---------[InventoryMessageConsumer]--------- Moved message to DLQ: {Encoding.UTF8.GetString(body)}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

    public class OrderMessage
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
