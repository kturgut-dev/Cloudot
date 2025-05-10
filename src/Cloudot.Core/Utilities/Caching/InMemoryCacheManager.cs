using Microsoft.Extensions.Caching.Memory;

namespace Cloudot.Core.Utilities.Caching;

public class InMemoryCacheManager(IMemoryCache cache) : ICacheManager
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        object? value = cache.Get(key);
        return Task.FromResult(value is T typed ? typed : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        cache.Set(key, value, duration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(cache.TryGetValue(key, out _));
    }

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(key, out object? cached) && cached is T value)
            return value;

        T? result = await factory.Invoke();
        if (result is not null)
            cache.Set(key, result, duration);

        return result;
    }
}