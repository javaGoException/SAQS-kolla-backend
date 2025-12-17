using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetPriorityRequest(
    [Required] Guid Guid,
    [Required] int Priority
);