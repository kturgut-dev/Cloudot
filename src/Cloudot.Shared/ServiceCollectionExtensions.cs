using Cloudot.Shared.Domain;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Repository.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCloudotShared(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork>();
        services.TryAddScoped<IEventBus, InMemoryEventBus>();

        return services;
    }
}