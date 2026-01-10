namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record AssignmentDto(
    string Guid,
    string DisplayName,
    string? Description,
    long Duration,
    long SequenceNumber,
    string? AssigneeGuid,
    string? RequiredRoleGuid,
    long Priority,
    long Status,
    string? ParentObjectiveGuid
);