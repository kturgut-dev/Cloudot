namespace Cloudot.Shared.Entity;

public class BaseTimestampEntity : BaseStatusEntity, ITimestampEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public BaseTimestampEntity() : base()
    {
        
    }
}