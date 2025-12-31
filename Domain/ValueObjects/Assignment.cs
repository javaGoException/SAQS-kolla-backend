using SAQS_kolla_backend.Domain.Enums;

namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Assignment
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public required string? Description {get; set;}
    public required DateTimeOffset? StartDate {get; set;}
    public required DateTimeOffset? EndDate {get; set;}
    public required DateTimeOffset? DeadlineDate {get; set;}
    public required Guid? AssigneeGuid {get; set;}
    public required Guid? RequiredRoleGuid {get; set;}
    public required Priority Priority { get; set; } = Priority.MidTerm;
    public required AssignmentStatus Status {get; set;} = AssignmentStatus.Planned;
    public required Guid? ParentObjectiveGuid {get; set;}
}