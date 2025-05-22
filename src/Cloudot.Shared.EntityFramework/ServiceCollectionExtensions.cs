using Cloudot.Shared.EntityFramework.Interceptor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Shared.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkShared(this IServiceCollection services)
    {
        // services.TryAddScoped<IUnitOfWork, EfUnitOfWork>();
        // services.TryAddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        // services.TryAddScoped(typeof(IEfRepository<>), typeof(EfRepository<,>));
        services.AddScoped<AuditSaveChangesInterceptor>();


        return services;
    }
}