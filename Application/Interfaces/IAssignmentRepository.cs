using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> QueryAssignment(Guid guid);
    Task<Assignment?> QueryAssignment(string name);
    Task<List<Guid>> QueryAllAssignmentsGuids();
    Task<int?> GetMaxSequenceNumber();
    Task<Guid?> GetNextAssignmentGuid(int sequenceNumber);
    Task<bool> InsertAssignment(Assignment assignment);
    Task<bool> UpdateDisplayName(Guid guid, string displayName);
    Task<bool> UpdateDescription(Guid guid, string? description);
    Task<bool> UpdateDuration(Guid guid, int duration);
    Task<bool> UpdateAssignee(Guid guid, Guid? assigneeGuid);
    Task<bool> UpdateRequiredRole(Guid guid, Guid? requiredRoleGuid);
    Task<bool> UpdatePriority(Guid guid, Priority priority);
    Task<bool> UpdateStatus(Guid guid, AssignmentStatus assignmentStatus);
    Task<bool> UpdateParentObjective(Guid guid, Guid? parentObjectiveGuid);
    Task<List<Assignment>> QueryAssignmentsByObjective(Guid parentObjectiveGuid);
    Task<bool> DeleteAssignment(Guid guid);
}