using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class ObjectiveService(IObjectiveRepository objectiveRepository) : IObjectiveService
{
    async Task<Result<List<Guid>>> IObjectiveService.GetAllGuids()
    {
        List<Guid> guids = await objectiveRepository.QueryAllObjectivesGuids();
        return Result<List<Guid>>.Success(guids);
    }

    async Task<Result<Objective>> IObjectiveService.Get(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result<Objective>.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }

        return Result<Objective>.Success(objective);
    }

    async Task<Result<Guid>> IObjectiveService.Create(string name, string? description)
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

    public async Task<Result> SetDisplayName(Guid guid, string displayName)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }

        await objectiveRepository.UpdateDisplayName(guid, displayName);

        return Result.Success();
    }

    public async Task<Result> SetDescription(Guid guid, string? description)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }

        await objectiveRepository.UpdateDescription(guid, description);

        return Result.Success();
    }

    async Task<Result> IObjectiveService.Delete(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }

        await objectiveRepository.DeleteObjective(guid);

        return Result.Success();
    }
}