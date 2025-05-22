using Cloudot.Infrastructure.Database.Interfaces;
using Npgsql;

namespace Cloudot.Infrastructure.Database.PostgreSQL;

public class PostgresDatabaseCreator(string adminConnectionString) : IDatabaseCreator
{
    public async Task CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
    {
        string commandText = $"CREATE DATABASE \"{databaseName}\";";

        await using NpgsqlConnection connection = new(adminConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using NpgsqlCommand command = new(commandText, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
