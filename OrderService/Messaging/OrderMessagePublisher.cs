using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderService.Model;

namespace OrderService.Messaging
{
    public class OrderMessagePublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IDictionary<string, TaskCompletionSource<string>> _pendingResponses;

        public OrderMessagePublisher()
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

            _replyQueueName = _channel.QueueDeclare().QueueName;
            _pendingResponses = new Dictionary<string, TaskCompletionSource<string>>();

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                if (_pendingResponses.TryGetValue(correlationId, out var tcs))
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.SetResult(response);
                }
            };

            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);
        }

        public async Task<string> PublishOrderAsync(Payload payload, TimeSpan timeout)
        {
            var correlationId = Guid.NewGuid().ToString();
            var taskCompletionSource = new TaskCompletionSource<string>();
            _pendingResponses[correlationId] = taskCompletionSource;

            var cancellationTokenSource = new CancellationTokenSource(timeout);
            cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);

            var message = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(message);

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;

            _channel.BasicPublish(exchange: "order_exchange",
                routingKey: "orderRK",
                basicProperties: props,
                body: body);

            Console.WriteLine($"Message sent with CorrelationId: {correlationId}");
            return await taskCompletionSource.Task;
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
