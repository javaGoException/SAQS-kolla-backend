namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorSetNicknameRequest(
    Guid Guid,
    string Nickname
);
