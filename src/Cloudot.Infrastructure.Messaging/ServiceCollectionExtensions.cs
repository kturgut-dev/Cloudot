using Cloudot.Infrastructure.Messaging.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        return services;
    }
}