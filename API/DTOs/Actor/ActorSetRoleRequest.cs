using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorSetRoleRequest(
    [Required] Guid Guid,
    Guid? RoleGuid
);
