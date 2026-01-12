using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveRepository
{
    Task<Objective?> QueryObjective(Guid guid);
    Task<Objective?> QueryObjective(string name);
    Task<List<Guid>> QueryAllObjectivesGuids(Guid? tenantId = null);
    Task<bool> InsertObjective(Objective objective);
    Task<bool> UpdateDisplayName(Guid guid, string displayName);
    Task<bool> UpdateDescription(Guid guid, string? description);
    Task<List<Guid>> QueryAllAssignments(Guid guid);
    Task<bool> DeleteObjective(Guid guid);
}