using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Objective;

public record ObjectiveSetDisplayNameRequest(
    [Required] Guid Guid,
    [Required] string DisplayName
);