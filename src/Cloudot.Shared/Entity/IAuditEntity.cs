using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public interface IAuditEntity : ITimestampEntity
{
    Guid? CreatedBy { get; set; }
    Guid? ModifiedBy { get; set; }
}