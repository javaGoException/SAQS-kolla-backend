using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Objective;

public record ObjectiveSetDescriptionRequest(
    [Required] Guid Guid,
    string? Description
);