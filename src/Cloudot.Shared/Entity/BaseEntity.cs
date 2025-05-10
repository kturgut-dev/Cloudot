using Cloudot.Shared.Domain;

namespace Cloudot.Shared.Entity;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; }
    
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