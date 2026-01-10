namespace SAQS_kolla_backend.Domain.ValueObjects;

public class Objective
{
    public required Guid Guid {get; set;}
    public required string DisplayName {get; set;}
    public required string? Description {get; set;}
    public required DateTimeOffset DeadlineDate {get; set;}
}