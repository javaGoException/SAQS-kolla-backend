using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Role;

public record RoleSetDescriptionRequest(
    [Required] Guid Guid,
    string? Description
);