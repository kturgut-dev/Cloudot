using Cloudot.Module.Management.Domain.Enums;
using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.TenantSubscription;

public class TenantSubscription : BaseAuditEntity
{
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } // Active, Suspended, Cancelled
    public decimal MonthlyPrice { get; set; } // Snapshot fiyat
    
    public virtual Tenant.Tenant Tenant { get; set; }
    public virtual Plan.Plan Plan { get; set; }
}