namespace Cloudot.Core.Tenant;

public interface ITenantStore
{
    Task<TenantConnectionInfo?> GetConnectionInfoByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
