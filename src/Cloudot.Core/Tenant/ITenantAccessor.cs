namespace Cloudot.Core.Tenant;

public interface ITenantAccessor
{
    Task<TenantConnectionInfo?> GetTenantInfoAsync(CancellationToken cancellationToken = default);
}
