using System.Data;
using Microsoft.Data.Sqlite;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class SqliteConnector : IDatabaseConnector
{

    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");

    public async Task<IDbConnection> OpenConnectionAsync()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new ArgumentNullException("The connection string was null or empty");
        }
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}