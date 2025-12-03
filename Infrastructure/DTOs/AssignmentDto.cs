namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record AssignmentDto(
    string DisplayName,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    DateTime DeadlineDateTime,
    string RequiredRole,
    string Priority,
    string AssignmentState
);