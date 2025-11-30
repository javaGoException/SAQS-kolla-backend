using Application.DTOs;
using Domain.ValueObjects;
using Infrastructure.Services;

namespace Application.Services;

public class ObjectiveService : IObjectiveService
{
    private IDatabaseManager _databaseManager = null!;
    public ObjectiveService(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }
    public async Task<CreateObjectiveDTO> CreateObjective(string name, string description)
    {
        Objective objective = new()
        {
            DisplayName = name,
            Description = description
        };
        await _databaseManager.InsertObjective(objective);

        CreateObjectiveDTO createObjectiveDTO = new(
            objective.Guid
        );
        return createObjectiveDTO;
    }
}