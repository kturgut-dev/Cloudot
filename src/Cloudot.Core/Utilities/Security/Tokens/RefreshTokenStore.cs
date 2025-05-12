using Cloudot.Core.Utilities.Caching;

namespace Cloudot.Core.Utilities.Security.Tokens;

public class RefreshTokenStore(ICacheManager _cacheManager) : IRefreshTokenStore
{
    private static string GetKey(string token) => $"refresh:{token}";

    public async Task StoreAsync(RefreshTokenInfo tokenInfo, CancellationToken cancellationToken = default)
    {
        TimeSpan ttl = tokenInfo.Expiration - DateTime.UtcNow;
        await _cacheManager.SetAsync(GetKey(tokenInfo.Token), tokenInfo, ttl, cancellationToken);
    }

    public async Task<RefreshTokenInfo?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _cacheManager.GetAsync<RefreshTokenInfo>(GetKey(token), cancellationToken);
    }

    public async Task RemoveAsync(string token, CancellationToken cancellationToken = default)
    {
        await _cacheManager.RemoveAsync(GetKey(token), cancellationToken);
    }
}