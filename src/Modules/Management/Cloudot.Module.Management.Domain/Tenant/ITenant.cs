using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.Tenant;

public interface ITenant : IAuditEntity
{
    string Name { get; set; }
    string ShortName { get; set; }
    string? Domain { get; set; } // e.g. "cloudot.example.com"
    string? Description { get; set; }
    string TimeZone { get; set; } // e.g. "UTC+8"
    string DatabaseName { get; set; }
    bool EnableSupport { get; set; }
    
    bool IsActive { get; set; } // Kullanımda mı?
    
    Guid? OwnerId { get; set; }
    Guid? PlanId { get; set; }
    
    User.User Owner { get; set; }
}