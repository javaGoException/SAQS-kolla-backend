using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Role;

public record RoleSetDisplayNameRequest(
    [Required] Guid Guid,
    [Required] string DisplayName
);