using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SAQS_kolla_backend.Options;

namespace SAQS_kolla_backend.Infrastructure.Setup;

public class SqliteConnector(IOptions<DatabaseOptions> options) : IDatabaseConnector
{
    private readonly string _connectionString = options.Value.SqliteConnectionString;

    async Task<IDbConnection> IDatabaseConnector.OpenConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}