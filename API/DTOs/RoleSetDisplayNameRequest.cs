namespace SAQS_kolla_backend.API.DTOs;

public record RoleSetDisplayNameRequest(
    string? Guid,
    string? DisplayName
);