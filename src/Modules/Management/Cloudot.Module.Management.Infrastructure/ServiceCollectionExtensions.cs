using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Seeding;
using Cloudot.Module.Management.Infrastructure.Localization;
using Cloudot.Module.Management.Infrastructure.Services;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.EntityFramework.Seeding;
using Cloudot.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Cloudot.Module.Management.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<ManagementDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<BaseDbContext, ManagementDbContext>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork<ManagementDbContext>>();

        services.TryAddScoped<ILocalizationEfRecordRepository, LocalizationEfRecordRepository>();
        services.TryAddScoped<ILocalizationRecordService, LocalizationRecordService>();

        services.AddManagementLocalization();


        services.TryAddScoped<IUserEfRepository, UserEfRepository>();
        services.TryAddScoped<IUserService, UserService>();
        services.TryAddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddManagementLocalization(this IServiceCollection services)
    {
        services.AddSingleton<DbStringLocalizer>();
        services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
        services.AddScoped(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        return services;
    }
}