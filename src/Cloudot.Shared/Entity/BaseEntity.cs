using Cloudot.Shared.Domain;
using MassTransit;

namespace Cloudot.Shared.Entity;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; } = NewId.NextGuid();

    public List<IDomainEvent> DomainEvents { get; } = new();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}