using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class ObjectiveService(IObjectiveRepository objectiveRepository) : IObjectiveService
{
    public async Task<Result<List<Guid>>> GetAllObjectivesGuid()
    {
        List<Guid> guids = await objectiveRepository.QueryAllObjectivesGuids();
        return Result<List<Guid>>.Success(guids);
    }

    public async Task<Result<Objective>> GetObjective(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result<Objective>.Failure(ResultError.NotFound, $"There is no objective with guid: {guid}");
        }

        return Result<Objective>.Success(objective);
    }

    public async Task<Result<Guid>> CreateObjective(string name, string description)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result<Guid>.Failure(ResultError.ValidationError, "Objective name cannot be empty");
        }

        Objective objective = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = name,
            Description = description
        };
        await objectiveRepository.InsertObjective(objective);

        return Result<Guid>.Success(objective.Guid);
    }

    public async Task<Result<Guid>> DeleteObjective(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result<Objective>.Failure(ResultError.NotFound, $"There is no objective with guid: {guid}");
        }

        await objectiveRepository.DeleteObjective(guid);

        return Result<Guid>.Success(objective.Guid);
    }
}