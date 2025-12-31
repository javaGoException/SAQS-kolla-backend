using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentCreateRequest(
    [Required] string DisplayName,
    [Required] string? Description,
    [Required] DateTimeOffset? StartDate,
    [Required] DateTimeOffset? DeadlineDate,
    [Required] Guid? AssigneeGuid,
    [Required] Guid? RequiredRoleGuid,
    [Required] Guid? ParentObjectiveGuid
);