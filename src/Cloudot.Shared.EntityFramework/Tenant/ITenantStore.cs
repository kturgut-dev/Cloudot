namespace Cloudot.Shared.EntityFramework.Tenant;

public interface ITenantStore
{
    Task<TenantConnectionInfo?> GetConnectionInfoByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
