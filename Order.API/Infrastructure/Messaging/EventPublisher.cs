using System.Text;
using System.Text.Json;
using Order.API.Application.Interfaces;
using RabbitMQ.Client;

namespace Order.API.Infrastructure.Messaging
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = "orders.exchange";
    }

    public class EventPublisher : IEventPublisher
    {
        private readonly RabbitMqConfiguration _config;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(RabbitMqConfiguration config, ILogger<EventPublisher> logger)
        {
            _config = config;
            _logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password
            };

            try
            {
                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                // Declare exchange
                _channel.ExchangeDeclareAsync(
                    exchange: config.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false
                ).Wait();

                _logger.LogInformation("RabbitMQ connection established");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ connection error");
            }
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            try
            {
                var eventName = @event.GetType().Name;
                var routingKey = $"order.{eventName.ToLower()}";

                var jsonMessage = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    Persistent = true,
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                await _channel.BasicPublishAsync(
                    exchange: _config.ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Event published: {eventName} with routing key: {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event");
            }
        }
    }
}
