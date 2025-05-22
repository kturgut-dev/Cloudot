using Cloudot.Infrastructure.Database.Interfaces;
using Cloudot.Infrastructure.Database.PostgreSQL;
using Cloudot.Shared.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Infrastructure.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseShared(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Creator
        services.AddScoped<IDatabaseCreator, PostgresDatabaseCreator>(sp =>
        {
            string connectionString = configuration.GetConnectionString("Master");
            return new PostgresDatabaseCreator(connectionString);
        });
        
        services.AddScoped<ITenantMigrationService, TenantMigrationService>();

        return services;
    }
}