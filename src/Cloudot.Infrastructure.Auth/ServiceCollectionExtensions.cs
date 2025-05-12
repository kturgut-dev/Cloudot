using Cloudot.Infrastructure.Auth.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services)
    {
        // services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();

        return services;
    }
}
