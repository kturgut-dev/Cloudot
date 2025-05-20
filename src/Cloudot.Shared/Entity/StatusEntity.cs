using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public class StatusEntity : BaseEntity, IStatusEntity
{
    public RecordStatus Status { get; set; }
}