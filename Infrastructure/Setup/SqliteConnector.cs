using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Setup;

public class SqliteConnector : IDatabaseConnector
{
    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");

    public async Task<DbConnection> OpenConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}