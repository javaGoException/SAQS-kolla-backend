using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetStartDateRequest(
    [Required] Guid Guid,
    DateTimeOffset? StartDate
);