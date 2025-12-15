using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveRepository
{
    Task<Objective?> QueryObjective(Guid guid);
    Task<Objective?> QueryObjective(string name);
    Task<List<Guid>> QueryAllObjectivesGuids();
    Task InsertObjective(Objective objective);
    Task<bool> UpdateDescription(Guid guid, string? description);
    Task DeleteObjective(Guid guid);
}