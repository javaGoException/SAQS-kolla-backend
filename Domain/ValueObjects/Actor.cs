namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Actor
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;} = string.Empty;
    public Role? Role {get; set;}
}