using Application.DTOs;
using Infrastructure.Services;

namespace Application.Services;

public class ObjectiveService : IObjectiveService
{
    private IDatabaseManager _databaseManager = null!;
    public ObjectiveService(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }
    public Task<CreateObjectiveDTO> CreateObjective(string name, string description)
    {
        throw new NotImplementedException();
    }
}