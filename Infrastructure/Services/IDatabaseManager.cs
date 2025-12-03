using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;

namespace SAQS_kolla_backend.Infrastructure.Services;

public interface IDatabaseManager
{
    Task<ObjectiveDto?> QueryObjective(Guid guid);
    Task<List<Guid>> QueryAllObjectivesGuids();
    Task InsertObjective(Objective objective);
}