namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorCreateRequest(
    string Nickname,
    Guid? RoleGuid
);
