using System.Reflection;
using System.Text;
using System.Text.Json;
using Cloudot.Shared.Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cloudot.Infrastructure.RabbitMQ;

// public class RabbitMqEventConsumer
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly IModel _channel;
//     private readonly IConnection _connection;
//
//     public RabbitMqEventConsumer(IServiceProvider serviceProvider, string hostName = "localhost")
//     {
//         _serviceProvider = serviceProvider;
//
//         ConnectionFactory factory = new ConnectionFactory
//         {
//             HostName = hostName,
//             DispatchConsumersAsync = true
//         };
//
//         _connection = factory.CreateConnection();
//         _channel = _connection.CreateModel();
//
//         _channel.ExchangeDeclare("domain_events", ExchangeType.Fanout);
//         string queueName = _channel.QueueDeclare().QueueName;
//         _channel.QueueBind(queue: queueName, exchange: "domain_events", routingKey: "");
//
//         AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
//         consumer.Received += OnEventReceived;
//
//         _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
//     }
//
//     private async Task OnEventReceived(object sender, BasicDeliverEventArgs eventArgs)
//     {
//         string json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
//
//         using IServiceScope scope = _serviceProvider.CreateScope();
//
//         // Tipi çözümle
//         JsonDocument doc = JsonDocument.Parse(json);
//         string? typeName = doc.RootElement.GetProperty("$type").GetString();
//         if (string.IsNullOrWhiteSpace(typeName)) return;
//
//         Type? eventType = Type.GetType(typeName);
//         if (eventType is null) return;
//
//         IDomainEvent? domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(json, eventType);
//         if (domainEvent is null) return;
//
//         Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
//         IEnumerable<object> handlers = scope.ServiceProvider.GetServices(handlerType);
//
//         foreach (object handler in handlers)
//         {
//             MethodInfo? handleMethod = handlerType.GetMethod("HandleAsync");
//             if (handleMethod is not null)
//             {
//                 await (Task)handleMethod.Invoke(handler, new object[] { domainEvent, CancellationToken.None })!;
//             }
//         }
//     }
//
//     public void Dispose()
//     {
//         _channel?.Dispose();
//         _connection?.Dispose();
//     }
// }
