using System.Text;
using Cloudot.Infrastructure.Auth.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Cloudot.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();
        
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        services.AddAuthorization();

        return services;
    }
}
