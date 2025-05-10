using System.Text.Json;
using Cloudot.Core.Utilities.Caching;
using StackExchange.Redis;

namespace Cloudot.Infrastructure.Redis;

public class RedisCacheManager(IConnectionMultiplexer redis) : ICacheManager
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        RedisValue value = await _db.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, duration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        T? existing = await GetAsync<T>(key, cancellationToken);
        if (existing is not null)
            return existing;

        T? result = await factory();
        if (result is not null)
            await SetAsync(key, result, duration, cancellationToken);

        return result;
    }
}
