using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveRepository
{
    Task<Objective?> QueryObjective(Guid guid);
    Task<List<Guid>> QueryAllObjectivesGuids();
    Task InsertObjective(Objective objective);
}