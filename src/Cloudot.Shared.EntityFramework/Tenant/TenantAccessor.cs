using Cloudot.Core.Utilities.Caching;
using Cloudot.Infrastructure.Auth;

namespace Cloudot.Shared.EntityFramework.Tenant;

public class TenantAccessor(
    ICurrentUser currentUser,
    ICacheManager cacheManager,
    ITenantStore tenantStore) : ITenantAccessor
{
    public async Task<TenantConnectionInfo?> GetTenantInfoAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.Id is null)
            return null;

        string cacheKey = CacheKeyHelper.User(currentUser.Id.Value);

        TenantConnectionInfo? cached = await cacheManager.GetAsync<TenantConnectionInfo>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        TenantConnectionInfo? tenantInfo = await tenantStore.GetConnectionInfoByUserIdAsync(currentUser.Id.Value, cancellationToken);
        if (tenantInfo is null)
            return null;

        await cacheManager.SetAsync(cacheKey, tenantInfo, TimeSpan.FromHours(1), cancellationToken);
        return tenantInfo;
    }
}
