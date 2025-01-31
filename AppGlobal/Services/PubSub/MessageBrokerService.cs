using System.Text;
using System.Text.Json;
using AppGlobal.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AppGlobal.Services.PubSub;

public class MessageBrokerService : IMessageBrokerService
{
    
        private readonly IConnectionFactory _factory;
        private readonly AppSettings _appsettings;
        private readonly ILogger<MessageBrokerService> _logger;
        public MessageBrokerService(ConnectionFactory factory,IOptions<AppSettings> appsettings, ILogger<MessageBrokerService> logger)
        {
            _factory = factory;
            _appsettings = appsettings.Value;
            _logger = logger;
        }

        public async Task RabbitPublish<T>(T data, string Operation = "OnaxAppDefault", CancellationToken ct = default!)
        {
            using var connection = await _factory.CreateConnectionAsync(ct);
            using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
            var props = new BasicProperties();
            await channel.QueueDeclareAsync("onaxapp-queue", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: ct);

            byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            await channel.BasicPublishAsync("onaxapp-exchange", routingKey: "onxe", mandatory: false, props, body: body, cancellationToken: ct);
        }

}

public interface IMessageBrokerService
{
    Task RabbitPublish<T>(T data, string Operation = "OnaxAppDefault", CancellationToken ct = default);
}
