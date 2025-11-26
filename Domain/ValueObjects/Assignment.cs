using Domain.Enums;

namespace Domain.ValueObjects;

public class Assignment
{
    public string DisplayName {get; set;} = "Empty Name";
    public string Description {get; set;} = "Empty Description";
    public DateTime StartDateTime {get; set;}
    public DateTime EndDateTime {get; private set;}
    public DateTime DeadlineDateTime {get; set;}
    public Role RequiredRole {get; set;} = new();
    public Priority Priority {get; set;}
}