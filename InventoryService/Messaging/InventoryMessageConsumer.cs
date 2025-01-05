using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using InventoryService.Model;
using InventoryService.Service;

namespace InventoryService.Messaging
{
    public class InventoryMessageConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ProductService _productService;

        public InventoryMessageConsumer(ProductService productService)
        {
            _productService = productService;
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "order_exchange", type: ExchangeType.Direct);
            _channel.QueueDeclare(queue: "order_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "order_queue", exchange: "order_exchange", routingKey: "orderRK");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Listen for message
                // calls RespondToOrder
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var payload = JsonSerializer.Deserialize<Payload>(message);

                        Console.WriteLine($"Received Order request: {payload.OrderDto.OrderId}");

                        var isStockAvailable = await CheckInventoryAsync(payload);
                        RespondToOrder(ea.BasicProperties, isStockAvailable);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _channel.BasicPublish(exchange: "dlx",
                            routingKey: "",
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(e.Message));
                    }
                    
                    
                };

                _channel.BasicConsume(queue: "order_queue", autoAck: false, consumer: consumer);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _channel.BasicPublish(exchange: "dlx",
                    routingKey: "",
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(e.Message));
                return Task.CompletedTask;
            }
            
        }

        private async Task<bool> CheckInventoryAsync(Payload payload)
        {
            Console.WriteLine($"CheckInventoryAsync: Payload Quantity - {payload.Quantity.ToString()}");
            return await _productService.HandleOrderCreated(payload);
        }

        private void RespondToOrder(IBasicProperties requestProperties, bool isStockAvailable)
        {
            if (string.IsNullOrEmpty(requestProperties.ReplyTo) ||
                string.IsNullOrEmpty(requestProperties.CorrelationId))
            {
                Console.WriteLine("-------[InventoryMessageConsumer]------ Missing ReplyTo or CorrelationId. Cannot send response.");
                return;
            }

            var response = isStockAvailable ? "Order Confirmed" : "Insufficient Stock";
            var responseBytes = Encoding.UTF8.GetBytes(response);

            var responseProps = _channel.CreateBasicProperties();
            responseProps.CorrelationId = requestProperties.CorrelationId;
            
            Console.WriteLine($"-------[InventoryMessageConsumer]---- Sending response: {response} with CorrelationId: {requestProperties.CorrelationId}");

            _channel.BasicPublish(exchange: "", routingKey: requestProperties.ReplyTo, basicProperties: responseProps, body: responseBytes);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
