using SAQS_kolla_backend.Domain.Enums;

namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Assignment
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public string? Description {get; set;}
    public DateTimeOffset? StartDate {get; set;}
    public DateTimeOffset? EndDate {get; set;}
    public DateTimeOffset? DeadlineDate {get; set;}
    public string? AssigneeGuid {get; set;}
    public string? RequiredRoleGuid {get; set;}
    public Priority Priority { get; set; } = Priority.MidTerm;
    public AssignmentStatus Status {get; set;} = AssignmentStatus.Planned;
}