namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Actor
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;} = string.Empty;
    public required Role? Role {get; set;}
    public Guid? TenantId {get; set;}
}