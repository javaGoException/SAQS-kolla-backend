using Dapper;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Application.Interfaces;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class ObjectiveRepository(IDatabaseConnector databaseConnector) : IObjectiveRepository
{
    public async Task<Objective?> QueryObjective(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Objectives o WHERE o.Guid = @Guid;";

        ObjectiveDto? objectiveDto = await connection.QuerySingleOrDefaultAsync<ObjectiveDto>(sql, new {Guid = guid});

        if (objectiveDto == null)
        {
            return null;
        }

        Objective objective = new()
        {
            Guid = Guid.Parse(objectiveDto.Guid),
            DisplayName = objectiveDto.DisplayName,
            Description = objectiveDto.Description
        };
        return objective;
    }

    public async Task<Objective?> QueryObjective(string name)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Objectives o WHERE o.DisplayName = @Name;";

        ObjectiveDto? objectiveDto = await connection.QuerySingleOrDefaultAsync<ObjectiveDto>(sql, new {Name = name});

        if (objectiveDto == null)
        {
            return null;
        }

        Objective objective = new()
        {
            Guid = Guid.Parse(objectiveDto.Guid),
            DisplayName = objectiveDto.DisplayName,
            Description = objectiveDto.Description
        };
        return objective;
    }

    public async Task<List<Guid>> QueryAllObjectivesGuids()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Objectives;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        List<Guid> guids = stringGuids.Select(g => Guid.Parse(g)).ToList();
        return guids;
    }

    public async Task InsertObjective(Objective objective)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description) VALUES (@Guid, @DisplayName, @Description);";

        await connection.ExecuteAsync(sql, objective);
    }

    public async Task<bool> UpdateDescription(Guid guid, string? description)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Objectives SET Description = @Description WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, Description = description});
        return affectedRows > 0;
    }

    public async Task<bool> UpdateDisplayName(Guid guid, string? displayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Objectives SET DisplayName = @Name WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, Name = displayName});
        return affectedRows > 0;
    }

    public async Task DeleteObjective(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "DELETE FROM Objectives WHERE Guid = @Guid;";

        await connection.ExecuteAsync(sql, new {Guid = guid});
    }
}