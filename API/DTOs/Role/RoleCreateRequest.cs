using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Role;

public record RoleCreateRequest(
    [Required] string DisplayName,
    [Required] string? Description,
    [Required] bool IsAdmin
);