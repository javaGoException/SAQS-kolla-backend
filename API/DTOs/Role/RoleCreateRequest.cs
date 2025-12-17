using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Role;

public record RoleCreateRequest(
    [Required] string DisplayName,
    string? Description,
    [Required] bool isAdmin
);