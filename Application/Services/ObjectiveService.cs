using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class ObjectiveService(IObjectiveRepository objectiveRepository) : IObjectiveService
{
    public async Task<Result<List<Guid>>> GetAllGuids()
    {
        List<Guid> guids = await objectiveRepository.QueryAllObjectivesGuids();
        return Result<List<Guid>>.Success(guids);
    }

    public async Task<Result<Objective>> Get(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result<Objective>.Failure(ResultError.NotFound, $"There is no objective with guid: {guid}");
        }

        return Result<Objective>.Success(objective);
    }

    public async Task<Result<Guid>> Create(string name, string? description)
    {
        Objective? existingObjective = await objectiveRepository.QueryObjective(name);

        if (existingObjective != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict, "The objective with this name already exists");
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

    public async Task<Result> SetObjectiveDescription(Guid guid, string? description)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, $"There is no objective with guid: {guid}");
        }

        await objectiveRepository.UpdateDescription(guid, description);

        return Result.Success();
    }

    public async Task<Result> Delete(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, $"There is no objective with guid: {guid}");
        }

        await objectiveRepository.DeleteObjective(guid);

        return Result.Success();
    }
}