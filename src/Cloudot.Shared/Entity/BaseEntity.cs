using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cloudot.Shared.Domain;

namespace Cloudot.Shared.Entity;

public abstract class BaseEntity : IEntity
{
    [Key]
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [ConcurrencyCheck]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public uint Version { get; set; }

    private List<IDomainEvent> DomainEvents { get; } = new();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
    
    public List<IDomainEvent> GetDomainEvents()
    {
        return this.DomainEvents;
    }
}