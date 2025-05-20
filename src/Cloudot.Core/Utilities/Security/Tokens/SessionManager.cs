using Cloudot.Core.Utilities.Caching;
using Cloudot.Core.Utilities.Security.Sessions;

namespace Cloudot.Core.Utilities.Security.Tokens;

public class SessionManager(ICacheManager _cacheManager) : ISessionManager
{
    private static string GetKey(string userId) => $"session:{userId}";

    public async Task SetAsync(string userId, SessionInfo session, CancellationToken cancellationToken = default)
    {
        await _cacheManager.SetAsync(GetKey(userId), session, TimeSpan.FromHours(8), cancellationToken);
    }

    public async Task<SessionInfo?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _cacheManager.GetAsync<SessionInfo>(GetKey(userId), cancellationToken);
    }

    public async Task RemoveAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _cacheManager.RemoveAsync(GetKey(userId), cancellationToken);
    }
}