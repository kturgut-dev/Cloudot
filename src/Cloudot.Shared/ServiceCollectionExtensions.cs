using Cloudot.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cloudot.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCloudotShared(this IServiceCollection services)
    {
        services.TryAddScoped<IEventBus, InMemoryEventBus>();

        return services;
    }
}