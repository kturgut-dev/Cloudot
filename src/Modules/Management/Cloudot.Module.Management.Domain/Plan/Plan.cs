using Cloudot.Module.Management.Domain.Enums;
using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.Plan;

public class Plan : BaseAuditEntity, IPlan
{
    public string Code { get; set; } = null!; // "FREE", "BASIC", "PRO"
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public bool IsFree { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPopular { get; set; }
    public PlanStatus Status { get; set; } 
    public int SortOrder { get; set; }

    public Plan() : base()
    {
        Status = PlanStatus.Active;
    }

    // Navigation properties
    public virtual ICollection<PlanLimit.PlanLimit> Limits { get; set; }
    public virtual ICollection<PlanModule.PlanModule> Modules { get; set; }
    public virtual ICollection<TenantSubscription.TenantSubscription> Subscriptions { get; set; }
}