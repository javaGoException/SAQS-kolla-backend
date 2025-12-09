using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SAQS_kolla_backend.Options;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class SqliteConnector(IOptions<DatabaseOptions> options) : IDatabaseConnector
{
    private readonly string _connectionString = options.Value.SqliteConnectionString;

    public async Task<IDbConnection> OpenConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}