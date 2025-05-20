namespace Cloudot.Shared.EntityFramework.Tenant;

public class TenantConnectionInfo
{
    public Ulid Id { get; set; }
    public string DbName { get; set; } = default!;
}