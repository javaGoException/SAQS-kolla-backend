using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetDurationRequest(
    [Required] Guid Guid,
    [Required] int Duration
);