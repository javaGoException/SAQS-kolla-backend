using Dapper;
using Microsoft.Data.Sqlite;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class SqliteManager : IDatabaseManager
{
    private async Task<SqliteConnection> OpenConnectionAsync()
    {
        string? connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("The connection string was null or empty");
        }
        var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<ObjectiveDto?> QueryObjective(Guid guid)
    {
        using var connection = await OpenConnectionAsync();
        string sql = "SELECT * FROM Objectives o WHERE o.Guid = @Guid;";

        ObjectiveDto? objectiveDto = await connection.QuerySingleOrDefaultAsync<ObjectiveDto>(sql, new {Guid = guid});
        return objectiveDto;
    }

    public async Task<List<Guid>> QueryAllObjectivesGuids()
    {
        using var connection = await OpenConnectionAsync();
        string sql = "SELECT Guid FROM Objectives;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        List<Guid> guids = stringGuids.Select(g => Guid.Parse(g)).ToList();
        return guids;
    }

    public async Task InsertObjective(Objective objective)
    {
        using var connection = await OpenConnectionAsync();
        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description) VALUES (@Guid, @DisplayName, @Description);";

        await connection.ExecuteAsync(sql, new {
            objective.Guid, 
            objective.DisplayName, 
            objective.Description
        });
    }
}