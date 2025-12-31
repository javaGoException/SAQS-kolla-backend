namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record AssignmentDto(
    string Guid,
    string DisplayName,
    string? Description,
    string? StartDate,
    string? EndDate,
    string? DeadlineDate,
    string? AssigneeGuid,
    string? RequiredRoleGuid,
    long Priority,
    long Status,
    string? ParentObjectiveGuid
);