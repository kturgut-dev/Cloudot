using System.ComponentModel.DataAnnotations;

namespace Cloudot.Shared.Entity;

public interface IEntity
{
    Ulid Id { get; set; }
    uint Version { get; set; }
}