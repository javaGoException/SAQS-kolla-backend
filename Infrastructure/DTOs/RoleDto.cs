namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record RoleDto(
    string Guid, 
    string DisplayName, 
    string? Description,
    bool IsAdmin
);
