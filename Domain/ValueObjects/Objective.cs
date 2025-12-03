namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Objective
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public string? Description {get; set;}
}