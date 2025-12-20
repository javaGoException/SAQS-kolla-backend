namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record ActorDto(
    string Guid,
    string DisplayName,
    string? RoleGuid
);
