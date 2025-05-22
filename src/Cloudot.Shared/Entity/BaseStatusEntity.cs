using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public class BaseStatusEntity : BaseEntity, IStatusEntity
{
    public RecordStatus Status { get; set; }

    public BaseStatusEntity() : base()
    {
        
    }
}