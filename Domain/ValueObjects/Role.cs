namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Role
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public required string? Description {get; set;}
    public required bool IsAdmin {get; set;}
    public Guid? TenantId {get; set;}
}