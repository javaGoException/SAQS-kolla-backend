namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorSetRoleRequest(
    Guid Guid,
    Guid? RoleGuid
);
