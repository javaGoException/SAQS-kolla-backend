using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetDeadlineDateRequest(
    [Required] Guid Guid,
    DateTimeOffset? DeadlineDate
);