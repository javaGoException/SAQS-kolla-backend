namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Actor
{
    public Guid Guid {get; private init;} = Guid.NewGuid();
    public string DisplayName {get; set;} = string.Empty;
    public Role Role {get; set;} = new();
    public List<Assignment> Assignments {get; set;} = new();
}