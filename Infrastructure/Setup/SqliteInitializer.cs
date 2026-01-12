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
        await EnsureColumnExists("Objectives", "TenantId", "TEXT");
        
        await CreateRolesTable();
        await EnsureColumnExists("Roles", "TenantId", "TEXT");

        await CreateActorsTable();
        await EnsureColumnExists("Actors", "TenantId", "TEXT");

        await CreateAssignmentTable();
    }

    private async Task EnsureColumnExists(string tableName, string columnName, string columnDefinition)
    {
        using var connection = new SqliteConnection(_connectionString);
        var count = await connection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM pragma_table_info('{tableName}') WHERE name='{columnName}';");
            
        if (count == 0)
        {
            await connection.ExecuteAsync($"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};");
        }
    }

    private async Task CreateObjectivesTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Objectives(
        Guid TEXT PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        Description TEXT,
        DeadlineDate TEXT NOT NULL,
        TenantId TEXT
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
        IsAdmin INTEGER NOT NULL,
        TenantId TEXT
        );");
    }

    private async Task CreateActorsTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Actors(
        Guid TEXT PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        RoleGuid TEXT,
        TenantId TEXT
        );");
    }

    private async Task CreateAssignmentTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Assignments(
        Guid TEXT PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        Description TEXT,
        Duration INTEGER NOT NULL,
        SequenceNumber INTEGER NOT NULL,
        AssigneeGuid TEXT,
        RequiredRoleGuid TEXT,
        Priority INTEGER NOT NULL,
        Status INTEGER NOT NULL,
        ParentObjectiveGuid TEXT
        );");
    }
}
