using System.Globalization;
using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Shared.Domain;
using Cloudot.Shared.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
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
        
        services.AddScoped<IExceptionFactory, ExceptionFactory>();
        
        string[] supportedCultures = new[] { "tr-TR", "en-US" };

        services.Configure<RequestLocalizationOptions>(options =>
        {
            List<CultureInfo> cultures = supportedCultures
                .Select(culture => new CultureInfo(culture))
                .ToList();

            options.DefaultRequestCulture = new RequestCulture("tr-TR");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;

            options.RequestCultureProviders = new[]
            {
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });

        return services;
    }
}