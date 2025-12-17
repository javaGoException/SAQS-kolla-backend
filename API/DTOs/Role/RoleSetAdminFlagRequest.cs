using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Role;

public record RoleSetAdminFlagRequest(
    [Required] Guid Guid,
    [Required] bool isAdmin
);