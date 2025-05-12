namespace Cloudot.Shared.EntityFramework.Tenant;

public interface ITenantAccessor
{
    Task<TenantConnectionInfo?> GetTenantInfoAsync(CancellationToken cancellationToken = default);
}
