namespace Cloudot.Shared.EntityFramework.Tenant;

public interface ITenantStore
{
    Task<TenantConnectionInfo?> GetConnectionInfoByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
