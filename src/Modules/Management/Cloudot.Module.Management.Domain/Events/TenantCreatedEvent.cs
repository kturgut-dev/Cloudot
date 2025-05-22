using Cloudot.Shared.Domain;

namespace Cloudot.Module.Management.Domain.Events;

public class TenantCreatedEvent(Guid tenantId, string databaseName) : IDomainEvent
{
    public Guid TenantId { get; } = tenantId;
    public string DatabaseName { get; } = databaseName;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
