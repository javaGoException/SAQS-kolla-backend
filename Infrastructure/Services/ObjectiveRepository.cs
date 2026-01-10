using Dapper;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Infrastructure.Setup;
using System.Globalization;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class ObjectiveRepository(IDatabaseConnector databaseConnector) : IObjectiveRepository
{
    async Task<Objective?> IObjectiveRepository.QueryObjective(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Objectives o WHERE o.Guid = @Guid;";

        ObjectiveDto? objectiveDto = await connection.QuerySingleOrDefaultAsync<ObjectiveDto>(sql, new {Guid = guid});

        if (objectiveDto == null)
        {
            return null;
        }

        DateTimeOffset deadlineDateParsed = DateTimeOffset.ParseExact(objectiveDto.DeadlineDate,"yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        Objective objective = new()
        {
            Guid = Guid.Parse(objectiveDto.Guid),
            DisplayName = objectiveDto.DisplayName,
            Description = objectiveDto.Description,
            DeadlineDate = deadlineDateParsed
        };
        return objective;
    }

    async Task<Objective?> IObjectiveRepository.QueryObjective(string name)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Objectives o WHERE o.DisplayName = @Name;";

        ObjectiveDto? objectiveDto = await connection.QuerySingleOrDefaultAsync<ObjectiveDto>(sql, new {Name = name});

        if (objectiveDto == null)
        {
            return null;
        }

        DateTimeOffset deadlineDateParsed = DateTimeOffset.ParseExact(objectiveDto.DeadlineDate,"yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        Objective objective = new()
        {
            Guid = Guid.Parse(objectiveDto.Guid),
            DisplayName = objectiveDto.DisplayName,
            Description = objectiveDto.Description,
            DeadlineDate = deadlineDateParsed
        };
        return objective;
    }

    async Task<List<Guid>> IObjectiveRepository.QueryAllObjectivesGuids()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Objectives;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        List<Guid> guids = stringGuids.Select(g => Guid.Parse(g)).ToList();
        return guids;
    }

    async Task<bool> IObjectiveRepository.InsertObjective(Objective objective)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Objectives(Guid, DisplayName, Description, DeadlineDate) VALUES (@Guid, @DisplayName, @Description, @DeadlineDate);";

        string parsedDeadline = objective.DeadlineDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        var param = new
        {
            Guid = objective.Guid,
            DisplayName = objective.DisplayName,
            Description = objective.Description,
            DeadlineDate = parsedDeadline
        };
        var affectedRows = await connection.ExecuteAsync(sql, param);
        
        return affectedRows > 0;
    }

    public async Task<bool> UpdateDisplayName(Guid guid, string displayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Objectives SET DisplayName = @Name WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, Name = displayName});
        return affectedRows > 0;
    }

    public async Task<bool> UpdateDescription(Guid guid, string? description)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Objectives SET Description = @Description WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, Description = description});
        return affectedRows > 0;
    }
    
    async Task<List<Guid>> IObjectiveRepository.QueryAllAssignments(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Assignments WHERE ParentObjectiveGuid = @ParentObjectiveGuid;";
        
        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql, new { ParentObjectiveGuid = guid });
        return stringGuids.Select(g => Guid.Parse(g)).ToList();
    }

    async Task<bool> IObjectiveRepository.DeleteObjective(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        
        string assignmentsSql = "UPDATE Assignments SET ParentObjectiveGuid = NULL WHERE ParentObjectiveGuid = @Guid;";
        var assignmentsAffectedRows = await connection.ExecuteAsync(assignmentsSql, new {Guid = guid});
        
        string objectiveSql = "DELETE FROM Objectives WHERE Guid = @Guid;";
        var objectiveAffectedRows = await connection.ExecuteAsync(objectiveSql, new {Guid = guid});
        
        return assignmentsAffectedRows > 0 && objectiveAffectedRows > 0;
    }
}