using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.LocalizationRecord;

public interface ILocalizationRecord : IEntity
{
    string? Culture { get; set; }
    string? Key { get; set; }
    string? Value { get; set; }
}