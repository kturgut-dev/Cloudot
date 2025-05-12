namespace Cloudot.Core.Utilities.Security.Sessions;

public interface ISessionManager
{
    Task SetAsync(Guid userId, SessionInfo session, CancellationToken cancellationToken = default);
    Task<SessionInfo?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid userId, CancellationToken cancellationToken = default);
}