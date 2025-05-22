using System.ComponentModel.DataAnnotations;

namespace Cloudot.Shared.Entity;

public interface IEntity
{
    Guid Id { get; set; }
    // Ulid Id { get; set; }
    uint Version { get; set; }
}