using Cloudot.Shared.Repository;
using Cloudot.Shared.Repository.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Shared.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkShared(this IServiceCollection services)
    {
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork>();
        services.TryAddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}