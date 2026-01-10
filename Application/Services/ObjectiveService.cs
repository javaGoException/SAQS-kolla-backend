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

    async Task<Result<Guid>> IObjectiveService.Create(string name, string? description, DateTimeOffset deadlineDate)
    {
        Objective? existingObjective = await objectiveRepository.QueryObjective(name);

        if (existingObjective != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict, "The objective with this name already exists");
        }

        if (deadlineDate < DateTimeOffset.Now)
        {
            return Result<Guid>.Failure(ResultError.ValidationError, "Deadline cannot be in the past");
        }
        
        Objective objective = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = name,
            Description = description,
            DeadlineDate = deadlineDate
        };
        await objectiveRepository.InsertObjective(objective);

        return Result<Guid>.Success(objective.Guid);
    }

    async Task<Result> IObjectiveService.SetDisplayName(Guid guid, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result.Failure(ResultError.ValidationError, "The display name is required");
        }
        
        Objective? objective = await objectiveRepository.QueryObjective(guid);
        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }
        
        Objective? duplicate = await objectiveRepository.QueryObjective(displayName);
        if (duplicate != null)
        {
            return Result.Failure(ResultError.Conflict, "The objective with this name already exist");
        }

        await objectiveRepository.UpdateDisplayName(guid, displayName);

        return Result.Success();
    }

    async Task<Result> IObjectiveService.SetDescription(Guid guid, string? description)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);

        if (objective == null)
        {
            return Result.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
        }

        await objectiveRepository.UpdateDescription(guid, description);

        return Result.Success();
    }
    
    async Task<Result<List<Guid>>> IObjectiveService.GetAllAssignments(Guid guid)
    {
        Objective? objective = await objectiveRepository.QueryObjective(guid);
        if (objective == null)
        {
            return Result<List<Guid>>.Failure(ResultError.NotFound, "Objective with this guid doesn't exist");
        }

        List<Guid> assignments = await objectiveRepository.QueryAllAssignments(guid);
        return Result<List<Guid>>.Success(assignments);
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