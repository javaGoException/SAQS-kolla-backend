namespace SAQS_kolla_backend.API.DTOs;

public record RoleCreateRequest(
    string? Name,
    string? Description
);