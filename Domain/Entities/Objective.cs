using Domain.ValueObjects;

namespace Domain.Entities;

public class Objective
{
    public string DisplayName {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public List<Assignment> Assignments = new();
}