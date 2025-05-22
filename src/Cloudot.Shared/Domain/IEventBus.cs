using Microsoft.EntityFrameworkCore;

namespace Cloudot.Shared.Domain;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent eventItem, CancellationToken cancellationToken = default);
    Task DispatchAsync(DbContext context, CancellationToken cancellationToken = default);
}