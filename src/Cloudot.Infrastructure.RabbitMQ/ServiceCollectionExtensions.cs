using Cloudot.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Infrastructure.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, string hostName = "localhost")
    {
        // services.AddSingleton<IEventBus>(_ => new RabbitMqEventBus(hostName));
        return services;
    }
} 