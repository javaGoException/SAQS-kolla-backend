namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Objective
{
    public Guid Guid {get; private init;} = Guid.NewGuid();
    public string DisplayName {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
}