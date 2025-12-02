using Microsoft.Data.Sqlite;
using Dapper;

namespace Infrastructure.Setup;

public class SqliteInitializer()
{
    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");
    public async Task InitializeDatabase()
    {
        await CreateObjectivesTable();
    }

    private async Task CreateObjectivesTable()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new ArgumentNullException("The connection string was null or empty");
        }
        
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS Objectives(
        Guid INTEGER PRIMARY KEY,
        DisplayName TEXT NOT NULL,
        Description TEXT
        );");
    }
}