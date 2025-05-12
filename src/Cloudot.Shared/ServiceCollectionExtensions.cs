using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Shared.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCloudotShared(this IServiceCollection services)
    {
        // Domain Event Bus
        services.TryAddScoped<IEventBus, InMemoryEventBus>();
        
        // Session ve Refresh Token Güvenliği
        services.TryAddScoped<ISessionManager, SessionManager>();
        services.TryAddScoped<IRefreshTokenStore, RefreshTokenStore>();

        // HTTP Context'e erişim
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}