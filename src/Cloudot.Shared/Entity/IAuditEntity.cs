using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public interface IAuditEntity : IEntity
{
    Guid? CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    Guid? ModifiedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
    RecordStatus Status { get; set; }
}