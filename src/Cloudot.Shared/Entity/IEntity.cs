using System.ComponentModel.DataAnnotations;
using Cloudot.Shared.Domain;

namespace Cloudot.Shared.Entity;

public interface IEntity
{
    Guid Id { get; set; }
    // Ulid Id { get; set; }
    uint Version { get; set; }
    
    void AddDomainEvent(IDomainEvent eventItem);
    void ClearDomainEvents();
    List<IDomainEvent> GetDomainEvents();
}