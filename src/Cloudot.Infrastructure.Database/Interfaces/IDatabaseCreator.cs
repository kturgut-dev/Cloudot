namespace Cloudot.Infrastructure.Database.Interfaces;

public interface IDatabaseCreator
{
    Task CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);
}
