using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public abstract class AuditEntity : BaseEntity, IAuditEntity
{
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Active;
}