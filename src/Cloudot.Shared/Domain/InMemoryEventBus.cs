using System.Reflection;
using Cloudot.Shared.Entity;
using Microsoft.EntityFrameworkCore;
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

    public async Task DispatchAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        List<IEntity> domainEntities = context.ChangeTracker
            .Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.GetDomainEvents().Any())
            .ToList();

        foreach (var entity in domainEntities)
        {
            List<IDomainEvent> events = entity.GetDomainEvents().ToList();
            entity.ClearDomainEvents();

            foreach (IDomainEvent domainEvent in events)
            {
                await PublishAsync(domainEvent, cancellationToken);
            }
        }
    }
}
