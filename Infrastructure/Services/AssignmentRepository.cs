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

        DateTimeOffset? startDateParsed = assignmentDto.StartDate == null ? null : DateTimeOffset.ParseExact(assignmentDto.StartDate,"yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        DateTimeOffset? endDateParsed = assignmentDto.EndDate == null ? null : DateTimeOffset.ParseExact(assignmentDto.EndDate,"yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        DateTimeOffset? deadlineDateParsed = assignmentDto.DeadlineDate == null ? null : DateTimeOffset.ParseExact(assignmentDto.DeadlineDate,"yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        Guid? assigneeGuidParsed = assignmentDto.AssigneeGuid == null ? null : Guid.Parse(assignmentDto.AssigneeGuid);
        Guid? requiredRoleGuid = assignmentDto.RequiredRoleGuid == null ? null : Guid.Parse(assignmentDto.RequiredRoleGuid);
        Guid? parentObjectiveGuid = assignmentDto.ParentObjectiveGuid == null ? null : Guid.Parse(assignmentDto.ParentObjectiveGuid);
        
        Assignment assignment = new()
        {
            Guid = Guid.Parse(assignmentDto.Guid),
            DisplayName = assignmentDto.DisplayName,
            Description = assignmentDto.Description,
            StartDate = startDateParsed,
            EndDate = endDateParsed,
            DeadlineDate = deadlineDateParsed,
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

    async Task<bool> IAssignmentRepository.InsertAssignment(Assignment assignment)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = @"
        INSERT INTO Assignments(Guid, DisplayName, Description, StartDate, EndDate, 
            DeadlineDate, AssigneeGuid, RequiredRoleGuid, Priority, Status, ParentObjectiveGuid)
        VALUES(@Guid,@DisplayName, @Description, @StartDate, @EndDate, 
            @DeadlineDate, @AssigneeGuid, @RequiredRoleGuid, @Priority, @Status, @ParentObjectiveGuid);";

        string? parsedStartDate = assignment.StartDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        string? parsedEndDate = assignment.EndDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        string? parsedDeadline = assignment.DeadlineDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            
        var param = new
        {
            Guid = assignment.Guid,
            DisplayName = assignment.DisplayName,
            Description = assignment.Description,
            StartDate = parsedStartDate,
            EndDate = parsedEndDate,
            DeadlineDate = parsedDeadline,
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

    async Task<bool> IAssignmentRepository.UpdateStartDate(Guid guid, DateTimeOffset? startDate)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET StartDate = @StartDate WHERE Guid = @Guid;";

        string? parsedStartDate = startDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, StartDate = parsedStartDate });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateEndDate(Guid guid, DateTimeOffset? endDate)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET EndDate = @EndDate WHERE Guid = @Guid;";
        
        string? parsedEndDate = endDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, EndDate = parsedEndDate });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.UpdateDeadlineDate(Guid guid, DateTimeOffset? deadlineDate)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Assignments SET DeadlineDate = @DeadlineDate WHERE Guid = @Guid;";

        string? parsedDeadlineDate = deadlineDate?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, DeadlineDate = parsedDeadlineDate });
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

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, ParentObjectiveGuid = parentObjectiveGuid });
        return affectedRows > 0;
    }

    async Task<bool> IAssignmentRepository.DeleteAssignment(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "DELETE FROM Assignments WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid});
        return affectedRows > 0;
    }
}