using Dapper;
using Microsoft.Data.Sqlite;
using Domain.ValueObjects;

namespace Infrastructure.Services;

public class SqliteManager : IDatabaseManager
{
    private async Task<SqliteConnection> OpenConnectionAsync()
    {
        string? _connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new ArgumentNullException("The connection string was null or empty");
        }
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task InsertObjective(Objective objective)
    {
        using var connection = await OpenConnectionAsync();

        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description) VALUES (@Guid, @DisplayName, @Description)";
        await connection.ExecuteAsync(sql, objective);
    }
}