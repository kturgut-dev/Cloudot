using Cloudot.Shared.Attribute;
using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.Tenant;

public class Tenant : BaseAuditEntity, ITenant
{
    public string Name { get; set; }
    
    [Slugify(nameof(Name))]
    public string ShortName { get; set; }
    public string? Domain { get; set; }
    public string? Description { get; set; }
    public string TimeZone { get; set; }
    public string DatabaseName { get; set; }
    public bool EnableSupport { get; set; }
    public bool IsActive { get; set; }
    
    public Guid? OwnerId { get; set; }
    public Guid? PlanId { get; set; }
    
    public virtual User.User Owner { get; set; }

    public Tenant() : base()
    {
        TimeZone = TimeZoneInfo.Local.StandardName;
        IsActive = true;
        EnableSupport = true;
    }
    
    public Tenant(string name, string shortName, string? domain, string? description, string culture, string timeZone, string databaseName) : this()
    {
        Name = name;
        ShortName = shortName;
        Domain = domain;
        Description = description;
        TimeZone = timeZone;
        DatabaseName = databaseName;
        IsActive = true;
        EnableSupport = true;
    }
}