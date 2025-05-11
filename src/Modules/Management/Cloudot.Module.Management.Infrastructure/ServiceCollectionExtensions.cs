using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;
using Cloudot.Module.Management.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Module.Management.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Management");

        services.AddDbContext<ManagementDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IUserService, UserService>();

        return services;
    }
}