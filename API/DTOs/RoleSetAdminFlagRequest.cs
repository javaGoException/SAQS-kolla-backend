namespace SAQS_kolla_backend.API.DTOs;

public record RoleSetAdminFlagRequest(
    string? Guid,
    bool isAdmin
);