using Domain.ValueObjects;

namespace Infrastructure.Services;

public interface IDatabaseManager
{
    Task InsertObjective(Objective objective);
}