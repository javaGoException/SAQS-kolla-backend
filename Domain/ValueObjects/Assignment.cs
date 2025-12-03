using SAQS_kolla_backend.Domain.Enums;

namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Assignment
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public string? Description {get; set;}
    public DateTime? StartDateTime {get; set;}
    public DateTime? EndDateTime {get; set;}
    public DateTime? DeadlineDateTime {get; set;}
    public Actor? Assignee {get; set;}
    public Role? RequiredRole {get; set;}
    public Priority Priority { get; set; } = Priority.MidTerm;
    public AssignmentState AssignmentState {get; set;} = AssignmentState.Planned;
}