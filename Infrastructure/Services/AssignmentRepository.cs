using System.Globalization;
using Dapper;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Infrastructure.Setup;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class AssignmentRepository(IDatabaseConnector databaseConnector) : IAssignmentRepository
{
    private async Task<Assignment?> QueryAssignmentWithSql(string sql, object parameters)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        AssignmentDto? assignmentDto = await connection.QuerySingleOrDefaultAsync<AssignmentDto>(sql, parameters);

        if (assignmentDto == null)
        {
            return null;
        }

        Guid? assigneeGuidParsed = assignmentDto.AssigneeGuid == null ? null : Guid.Parse(assignmentDto.AssigneeGuid);
        Guid? requiredRoleGuid = assignmentDto.RequiredRoleGuid == null ? null : Guid.Parse(assignmentDto.RequiredRoleGuid);
        Guid? parentObjectiveGuid = assignmentDto.ParentObjectiveGuid == null ? null : Guid.Parse(assignmentDto.ParentObjectiveGuid);
        
        Assignment assignment = new()
        {
            Guid = Guid.Parse(assignmentDto.Guid),
            DisplayName = assignmentDto.DisplayName,
            Description = assignmentDto.Description,
            Duration = (int) assignmentDto.Duration,
            SequenceNumber = (int) assignmentDto.SequenceNumber,
            AssigneeGuid = assigneeGuidParsed,
            RequiredRoleGuid = requiredRoleGuid,
            Priority = (Priority)assignmentDto.Priority,
            Status = (AssignmentStatus)assignmentDto.Status,
            ParentObjectiveGuid = parentObjectiveGuid
        };
        return assignment;
    }
    async Task<Assignment?> IAssignmentRepository.QueryAssignment(Guid guid)
    {
        string sql = "SELECT * FROM Assignments a WHERE a.Guid = @Guid;";
        return await QueryAssignmentWithSql(sql, new {Guid = guid});
    }

    async Task<Assignment?> IAssignmentRepository.QueryAssignment(string name)
    {
        string sql = "SELECT * FROM Assignments a WHERE a.DisplayName = @Name;";
        return await QueryAssignmentWithSql(sql, new {Name = name});
    }

    async Task<List<Guid>> IAssignmentRepository.QueryAllAssignmentsGuids()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Assignments;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        List<Guid> guids = stringGuids.Select(g => Guid.Parse(g)).ToList();
        return guids;
    }

    async Task<int?> IAssignmentRepository.GetMaxSequenceNumber()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        var result = await connection.ExecuteScalarAsync<int?>(@"SELECT MAX(SequenceNumber) FROM Assignments");
        return result;
    }

    async Task<Guid?> IAssignmentRepository.GetNextAssignmentGuid(int sequenceNumber)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = @"
        SELECT Guid FROM Assignments 
        WHERE SequenceNumber > @target
        ORDER BY SequenceNumber ASC 
        LIMIT 1";
        
        var guidString = await connection.ExecuteScalarAsync<string?>(sql, new { target = sequenceNumber });
        
        return guidString != null ? Guid.Parse(guidString) : null;
    }

    async Task<bool> IAssignmentRepository.InsertAssignment(Assignment assignment)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = @"
        INSERT INTO Assignments(Guid, DisplayName, Description, Duration, SequenceNumber, AssigneeGuid, RequiredRoleGuid, Priority, Status, ParentObjectiveGuid)
        VALUES(@Guid,@DisplayName, @Description, @Duration, @SequenceNumber, @AssigneeGuid, @RequiredRoleGuid, @Priority, @Status, @ParentObjectiveGuid);";
            
        var param = new
        {
            Guid = assignment.Guid,
            DisplayName = assignment.DisplayName,
            Description = assignment.Description,
            Duration = assignment.Duration,
            SequenceNumber = assignment.SequenceNumber,
            AssigneeGuid = assignment.AssigneeGuid,
            RequiredRoleGuid = assignment.RequiredRoleGuid,
            Priority = assignment.Priority,
            Status = assignment.Status,
            ParentObjectiveGuid = assignment.ParentObjectiveGuid
        };
        var affectedRows = await connection.ExecuteAsync(sql, param);
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateDisplayName(Guid guid, string displayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET DisplayName = @DisplayName WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, DisplayName = displayName});
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateDescription(Guid guid, string? description)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET Description = @Description WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, Description = description });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateDuration(Guid guid, int Duration)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET Duration = @Duration WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, Duration = Duration });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateAssignee(Guid guid, Guid? assigneeGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET AssigneeGuid = @AssigneeGuid WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, AssigneeGuid = assigneeGuid });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateRequiredRole(Guid guid, Guid? requiredRoleGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET RequiredRoleGuid = @RequiredRoleGuid WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, RequiredRoleGuid = requiredRoleGuid });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdatePriority(Guid guid, Priority priority)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET Priority = @Priority WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, Priority = priority });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateStatus(Guid guid, AssignmentStatus assignmentStatus)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET Status = @Status WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, Status = assignmentStatus });
        return affectedRows > 0;
    }
    
    async Task<bool> IAssignmentRepository.UpdateParentObjective(Guid guid, Guid? parentObjectiveGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET ParentObjectiveGuid = @ParentObjectiveGuid WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, ParentObjectiveGuid = parentObjectiveGuid});
        return affectedRows > 0;
    }

    async Task<List<Assignment>> IAssignmentRepository.QueryAssignmentsByObjective(Guid parentObjectiveGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Assignments WHERE ParentObjectiveGuid = @ParentObjectiveGuid;";
        IEnumerable<AssignmentDto> assignmentDtos = await connection.QueryAsync<AssignmentDto>(sql, new { ParentObjectiveGuid = parentObjectiveGuid });

        List<Assignment> assignments = new();
        foreach(var dto in assignmentDtos)
        {
            assignments.Add(new Assignment
            {
                Guid = Guid.Parse(dto.Guid),
                DisplayName = dto.DisplayName,
                Description = dto.Description,
                Duration = (int)dto.Duration,
                SequenceNumber = (int)dto.SequenceNumber,
                AssigneeGuid = dto.AssigneeGuid != null ? Guid.Parse(dto.AssigneeGuid) : null,
                RequiredRoleGuid = dto.RequiredRoleGuid != null ? Guid.Parse(dto.RequiredRoleGuid) : null,
                Priority = (Priority)dto.Priority,
                Status = (AssignmentStatus)dto.Status,
                ParentObjectiveGuid = dto.ParentObjectiveGuid != null ? Guid.Parse(dto.ParentObjectiveGuid) : null
            });
        }
        return assignments;
    }

    async Task<bool> IAssignmentRepository.DeleteAssignment(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "DELETE FROM Assignments WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid});
        return affectedRows > 0;
    }
}