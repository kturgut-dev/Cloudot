namespace Cloudot.Core.Utilities.Caching;

public interface ICacheManager
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory, TimeSpan duration, CancellationToken cancellationToken = default);
}