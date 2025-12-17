using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Domain.Enums;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IAssignmentService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Assignment>> Get(Guid guid);
    Task<Result<Guid>> Create(string DisplayName, string? Description, DateTimeOffset? StartDate, DateTimeOffset? DeadlineDate, Guid? AssigneeGuid, Guid? RequiredRole);
    Task<Result> SetDisplayName(Guid Guid, string DisplayName);
    Task<Result> SetDescription(Guid Guid, string? Description);
    Task<Result> SetStartDate(Guid Guid, DateTimeOffset? StartDate);
    Task<Result> SetDeadlineDate(Guid Guid, DateTimeOffset? DeadlineDate);
    Task<Result> SetAssignee(Guid Guid, Guid? AssigneeGuid);
    Task<Result> SetPriority(Guid Guid, Priority Priority);
    Task<Result> SetStatus(Guid Guid, AssignmentStatus AssignmentStatus);
    Task<Result> Delete(Guid guid);
}