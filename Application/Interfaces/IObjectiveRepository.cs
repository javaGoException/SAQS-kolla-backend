using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveRepository
{
    Task<Objective?> QueryObjective(Guid guid);
    Task<Objective?> QueryObjective(string name);
    Task<List<Guid>> QueryAllObjectivesGuids();
    Task<bool> InsertObjective(Objective objective);
    Task<bool> UpdateDisplayName(Guid guid, string displayName);
    Task<bool> UpdateDescription(Guid guid, string? description);
    Task<bool> DeleteObjective(Guid guid);
}