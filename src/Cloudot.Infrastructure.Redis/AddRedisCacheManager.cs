using Cloudot.Core.Utilities.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Cloudot.Infrastructure.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration["Redis:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Redis bağlantı cümlesi bulunamadı. 'Redis:ConnectionString' ayarını kontrol edin.");

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddScoped<ICacheManager, RedisCacheManager>();

        return services;
    }
}