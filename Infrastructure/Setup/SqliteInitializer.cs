using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Options;
using SAQS_kolla_backend.Options;

namespace SAQS_kolla_backend.Infrastructure.Setup;

public class SqliteInitializer(IOptions<DatabaseOptions> options)
{
    private readonly string _connectionString = options.Value.SqliteConnectionString;
    public async Task InitializeDatabase()
    {
        await CreateObjectivesTable();
        await CreateRolesTable();
    }

    private async Task CreateObjectivesTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Objectives(
        Guid TEXT PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        Description TEXT
        );");
    }

    private async Task CreateRolesTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Roles(
        Guid TEXT PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        Description TEXT,
        isAdmin INTEGER NOT NULL
        );");
    }
}