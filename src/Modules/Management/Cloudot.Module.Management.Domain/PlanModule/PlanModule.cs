using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.PlanModule;

public class PlanModule : BaseEntity
{
    public Guid PlanId { get; set; }
    public string ModuleName { get; set; } = null!; // "Procurement", "Finance", "HR"
    public bool IsIncluded { get; set; }
    public string? AccessLevel { get; set; } // "Basic", "Advanced", "Full" (opsiyonel)
    
    public virtual Plan.Plan Plan { get; set; } = null!;
}