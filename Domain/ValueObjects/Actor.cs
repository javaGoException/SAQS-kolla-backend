namespace Domain.ValueObjects;

public class Actor
{
    public Guid Guid {get; private init;} = new Guid();
    public string DisplayName {get; set;} = string.Empty;
    public Role Role {get; set;} = new();
    public List<Assignment> Assignments {get; set;} = new();
}