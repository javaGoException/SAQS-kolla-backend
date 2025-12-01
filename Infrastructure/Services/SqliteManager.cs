using Dapper;
using Domain.ValueObjects;
using Infrastructure.Setup;

namespace Infrastructure.Services;

public class SqliteManager : IDatabaseManager
{
    private readonly IDatabaseConnector _sqliteConnector;

    public SqliteManager(IDatabaseConnector sqliteConnector)
    {
        _sqliteConnector = sqliteConnector;
    }

    public async Task InsertObjective(Objective objective)
    {
        using var connection = await _sqliteConnector.OpenConnectionAsync();

        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description) VALUES (@Guid, @DisplayName, @Description)";
        await connection.ExecuteAsync(sql, new { objective.Guid, objective.DisplayName, objective.Description });
    }
}