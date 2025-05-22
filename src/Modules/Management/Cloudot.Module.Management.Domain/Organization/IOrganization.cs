using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.Organization;

public interface IOrganization : IAuditEntity
{
    string Name { get; set; }
    string ShortName { get; set; }
    string? Description { get; set; }
    string TimeZone { get; set; } // e.g. "UTC+8"
    string DatabaseName { get; set; }
    
    Ulid? OwnerId { get; set; }
}