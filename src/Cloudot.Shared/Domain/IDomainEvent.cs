namespace Cloudot.Shared.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}