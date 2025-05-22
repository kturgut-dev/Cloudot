using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public abstract class BaseAuditEntity : BaseTimestampEntity, IAuditEntity
{
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public BaseAuditEntity() : base()
    {
        
    }
}