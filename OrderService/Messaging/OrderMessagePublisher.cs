using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Messaging
{
    public class OrderMessagePublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OrderMessagePublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue with dead-letter exchange support
            _channel.QueueDeclare(queue: "order_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "dlx_exchange" }
                });
        }

        public void PublishOrder(int orderId, int productId, int quantity)
        {
            var message = JsonSerializer.Serialize(new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            });

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "exchange",
                routingKey: "order_queue",
                basicProperties: null,
                body: body);

            Console.WriteLine($"[OrderMessagePublisher] Message sent: {message}");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}