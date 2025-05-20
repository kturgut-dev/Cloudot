namespace Cloudot.Core.Utilities.Security.Sessions;

public interface ISessionManager
{
    Task SetAsync(string userId, SessionInfo session, CancellationToken cancellationToken = default);
    Task<SessionInfo?> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string userId, CancellationToken cancellationToken = default);
}