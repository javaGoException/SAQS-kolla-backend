using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Domain.Enums;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IAssignmentService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Assignment>> Get(Guid guid);
    Task<Result<Guid>> Create(string displayName, string? description, int duration, Guid? assigneeGuid, Guid? requiredRole, Guid? parentObjectiveGuid);
    Task<Result> SetDisplayName(Guid guid, string displayName);
    Task<Result> SetDescription(Guid guid, string? description);
    Task<Result> SetDuration(Guid guid, int duration);
    Task<Result> SetAssignee(Guid guid, Guid? assigneeGuid);
    Task<Result> SetRequiredRole(Guid guid, Guid? requiredRole);
    Task<Result> SetPriority(Guid guid, Priority priority);
    Task<Result> SetStatus(Guid guid, AssignmentStatus assignmentStatus);
    Task<Result> SetParentObjective(Guid guid, Guid? parentObjectiveGuid);
    Task<Result> Delete(Guid guid);
}