using Domain.ValueObjects;
using Infrastructure.DTOs;

namespace Infrastructure.Services;

public interface IDatabaseManager
{
    Task<ObjectiveDto?> QueryObjective(Guid guid);
    Task<List<Guid>> QueryAllObjectivesGuids();
    Task InsertObjective(Objective objective);
}