using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public interface IAuditEntity : IStatusEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
}