using System.Text;
using System.Text.Json;
using Cloudot.Shared.Domain;
using RabbitMQ.Client;

namespace Cloudot.Infrastructure.RabbitMQ;
//
// public class RabbitMqEventBus : IEventBus, IDisposable
// {
//     private readonly IConnection _connection;
//     private readonly IModel _channel;
//
//     public RabbitMqEventBus(string hostName = "localhost")
//     {
//         ConnectionFactory factory = new ConnectionFactory
//         {
//             HostName = hostName,
//             DispatchConsumersAsync = true
//         };
//
//         _connection = factory.CreateConnection();
//         _channel = _connection.CreateModel();
//
//         _channel.ExchangeDeclare(
//             exchange: "domain_events",
//             type: ExchangeType.Fanout,
//             durable: true,
//             autoDelete: false,
//             arguments: null
//         );
//     }
//
//     public Task PublishAsync(IDomainEvent eventItem, CancellationToken cancellationToken = default)
//     {
//         string eventName = eventItem.GetType().Name;
//         string message = JsonSerializer.Serialize(eventItem);
//         byte[] body = Encoding.UTF8.GetBytes(message);
//
//         _channel.BasicPublish(
//             exchange: "domain_events",
//             routingKey: string.Empty,
//             basicProperties: null,
//             body: body
//         );
//
//         return Task.CompletedTask;
//     }
//
//     public void Dispose()
//     {
//         _channel?.Dispose();
//         _connection?.Dispose();
//     }
// }