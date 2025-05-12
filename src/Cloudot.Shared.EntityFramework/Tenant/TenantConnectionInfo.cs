namespace Cloudot.Shared.EntityFramework.Tenant;

public class TenantConnectionInfo
{
    public Guid Id { get; set; }
    public string DbName { get; set; } = default!;
}