namespace Cloudot.Shared.Domain;

public interface IHasDomainEvents
{
    List<IDomainEvent> DomainEvents { get; }

    void AddDomainEvent(IDomainEvent eventItem);
    void ClearDomainEvents();
}