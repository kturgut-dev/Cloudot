namespace Cloudot.Shared.Entity;

public interface ITimestampEntity : IStatusEntity
{
    DateTime CreatedDate { get; set; }
    DateTime? ModifiedDate { get; set; }
}