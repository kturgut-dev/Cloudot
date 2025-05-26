using Cloudot.Module.Management.Domain.Enums;
using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.Plan;

public interface IPlan : IAuditEntity
{
    string Code { get; set; } 
    string Name { get; set; }
    string DisplayName { get; set; }
    string Description { get; set; }
    decimal MonthlyPrice { get; set; }
    decimal YearlyPrice { get; set; }
    bool IsFree { get; set; }
    bool IsDefault { get; set; } 
    bool IsPopular { get; set; }
    PlanStatus Status { get; set; }
    int SortOrder { get; set; }
}