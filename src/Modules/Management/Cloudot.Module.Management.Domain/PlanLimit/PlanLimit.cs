using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.PlanLimit;

public class PlanLimit: BaseEntity
{
    public Guid PlanId { get; set; }
    public string LimitType { get; set; } = null!; // "MAX_USERS", "MAX_PROJECTS", "MAX_SUPPLIERS", "STORAGE_GB"
    public int Value { get; set; } // -1 = unlimited
    public string? Description { get; set; } // "Maximum number of users"
    
    public virtual Plan.Plan Plan { get; set; }
}