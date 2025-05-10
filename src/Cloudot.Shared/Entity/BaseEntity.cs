namespace Cloudot.Shared.Entity;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; }
}