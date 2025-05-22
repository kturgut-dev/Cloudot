using Cloudot.Infrastructure.Database.Interfaces;
using Cloudot.Module.Management.Domain.Events;
using Cloudot.Shared.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cloudot.Module.Management.Application.EventHandlers;

public class TenantCreatedEventHandler(
    IDatabaseCreator databaseCreator,
    ITenantMigrationService tenantMigrationService,
    IConfiguration configuration,
    ILogger<TenantCreatedEventHandler> logger)
    : IDomainEventHandler<TenantCreatedEvent>
{
    public async Task HandleAsync(TenantCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await databaseCreator.CreateDatabaseAsync(domainEvent.DatabaseName, cancellationToken);

        string template = configuration.GetConnectionString("TenantTemplate");
        string connectionString = template.Replace("{DataBaseName}", domainEvent.DatabaseName);

        logger.LogInformation("Tenant DB oluşturuldu. Migration başlatılıyor: {Db}", domainEvent.DatabaseName);

        await tenantMigrationService.MigrateAllAsync(connectionString, cancellationToken);

        logger.LogInformation("Tenant DB migration tamamlandı: {Db}", domainEvent.DatabaseName);
    }
}