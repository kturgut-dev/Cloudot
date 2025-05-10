using Cloudot.Core.Utilities.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Cloudot.Infrastructure.Redis;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnection = configuration.GetConnectionString("Redis")
                                  ?? throw new InvalidOperationException("Redis connection string not found.");

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddScoped<ICacheManager, RedisCacheManager>();

        return services;
    }
} 
