using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetStatusRequest(
    [Required] Guid Guid,
    [Required] int AssignmentStatus
);