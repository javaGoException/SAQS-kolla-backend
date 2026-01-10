using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentCreateRequest(
    [Required] string DisplayName,
    string? Description,
    [Required] int Duration,
    Guid? AssigneeGuid,
    Guid? RequiredRoleGuid,
    Guid? ParentObjectiveGuid
);