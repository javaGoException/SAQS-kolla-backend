using SAQS_kolla_backend.Application.DTOs;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.Services;

namespace SAQS_kolla_backend.Application.Services;

public class ObjectiveService : IObjectiveService
{
    private IDatabaseManager _databaseManager = null!;
    public ObjectiveService(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<CreateObjectiveDto> CreateObjective(string name, string description)
    {
        Objective objective = new()
        {
            DisplayName = name,
            Description = description
        };
        await _databaseManager.InsertObjective(objective);

        CreateObjectiveDto createObjectiveDTO = new(
            objective.Guid
        );
        return createObjectiveDTO;
    }
}