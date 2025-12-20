namespace SAQS_kolla_backend.Infrastructure.DTOs;

public class RoleDto
{
    public required string Guid { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }
}
