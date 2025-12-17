using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentCreateRequest(
    [Required] string DisplayName,
    string? Description,
    DateTimeOffset? StartDate,
    DateTimeOffset? DeadlineDate,
    Guid? AssigneeGuid,
    Guid? RequiredRoleGuid
);