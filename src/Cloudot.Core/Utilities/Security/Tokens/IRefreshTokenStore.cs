namespace Cloudot.Core.Utilities.Security.Tokens;

public interface IRefreshTokenStore
{
    Task StoreAsync(RefreshTokenInfo tokenInfo, CancellationToken cancellationToken = default);
    Task<RefreshTokenInfo?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task RemoveAsync(string token, CancellationToken cancellationToken = default);
}