using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Shared.Domain;

public class InMemoryEventBus(IServiceProvider serviceProvider) : IEventBus
{
    public async Task PublishAsync(IDomainEvent eventItem, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventItem.GetType());
        IEnumerable<object> handlers = serviceProvider.GetServices(handlerType);

        foreach (object handler in handlers)
        {
            MethodInfo method = handlerType.GetMethod("HandleAsync")!;
            await (Task)method.Invoke(handler, new object[] { eventItem, cancellationToken })!;
        }
    }
}