using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetDescriptionRequest(
    [Required] Guid Guid,
    string? Description
);