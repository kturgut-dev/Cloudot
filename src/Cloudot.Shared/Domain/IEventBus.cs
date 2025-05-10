namespace Cloudot.Shared.Domain;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent eventItem, CancellationToken cancellationToken = default);
}