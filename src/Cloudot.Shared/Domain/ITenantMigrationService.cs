namespace Cloudot.Shared.Domain;

public interface ITenantMigrationService
{
    Task MigrateAllAsync(string connectionString, CancellationToken cancellationToken = default);
}
