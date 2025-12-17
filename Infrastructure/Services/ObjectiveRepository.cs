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

    public async Task<bool> InsertObjective(Objective objective)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description) VALUES (@Guid, @DisplayName, @Description);";

        var affectedRows = await connection.ExecuteAsync(sql, objective);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteObjective(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "DELETE FROM Objectives WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid});
        return affectedRows > 0;
    }
}