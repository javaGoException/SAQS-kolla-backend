using Domain.ValueObjects;

namespace Domain.Entities;

public class Actor
{
    public string DisplayName {get; set;} = string.Empty;
    public Role Role {get; set;} = new();
    public List<Assignment> Assignments {get; set;} = new();
}