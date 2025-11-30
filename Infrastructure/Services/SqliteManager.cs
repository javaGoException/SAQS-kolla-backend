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

        string sql = "INSERT INTO Objectives(guid, displayName, description) VALUES (@guid, @displayName, @description)";
        await connection.ExecuteAsync(sql, new { objective.Guid, objective.DisplayName, objective.Description });
    }
}