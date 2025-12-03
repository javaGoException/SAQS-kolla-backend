namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Role
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public string? Description {get; set;}
    public required bool IsAdmin {get; set;}
}