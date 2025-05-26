using Cloudot.Shared.Enums;

namespace Cloudot.Shared.Entity;

public interface IStatusEntity : IEntity
{
    RecordStatus RecordStatus { get; set; }
}