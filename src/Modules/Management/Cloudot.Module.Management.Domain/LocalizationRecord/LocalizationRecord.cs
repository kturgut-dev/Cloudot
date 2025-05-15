using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.LocalizationRecord;

public class LocalizationRecord : BaseEntity, ILocalizationRecord
{
    public string? Culture { get; set; } = null!;
    public string? Key { get; set; } = null!;
    public string? Value { get; set; } = null!;
}