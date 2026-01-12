using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Objective;

public record ObjectiveCreateRequest(
    [Required] string DisplayName,
    [Required] DateTimeOffset DeadlineDate,
    string? Description,
    Guid? TenantId
);