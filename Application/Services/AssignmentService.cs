using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class AssignmentService(IAssignmentRepository assignmentRepository, IActorRepository actorRepository, IRoleRepository roleRepository, IObjectiveRepository objectiveRepository) : IAssignmentService
{
    private Priority CalculateAssignmentPriority(DateTimeOffset deadlineDate, int duration)
    {
        int hoursUntilDeadline = (int)(deadlineDate - DateTimeOffset.Now).TotalHours;
        int remainingTimeAfterWork = hoursUntilDeadline - duration; 

        if (remainingTimeAfterWork <= 8)
        {
            return Priority.ShortTerm;
        }

        return remainingTimeAfterWork <= 32 ? Priority.MidTerm : Priority.LongTerm;
    }

    private async Task<bool> DoesAssigneeHaveRequiredRole(Guid? assigneeGuid, Guid? requiredRoleGuid)
    {
        if (requiredRoleGuid == null)
        {
            return true;
        }

        if (assigneeGuid == null)
        {
            return false;
        }
        
        Actor? actor = await actorRepository.QueryActor(assigneeGuid.Value);
        if (actor == null)
        {
            return false;
        }
        
        if (actor.Role == null)
        {
            return false;
        }
        
        if (actor.Role.Guid != requiredRoleGuid)
        {
            return false;
        }
        
        return true;
    }

    async Task<Result<List<Guid>>> IAssignmentService.GetAllGuids()
    {
        List<Guid> guids = await assignmentRepository.QueryAllAssignmentsGuids();
        return Result<List<Guid>>.Success(guids);
    }
    
    async Task<Result<Assignment>> IAssignmentService.Get(Guid guid)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        
        if (assignment == null)
        {
            return Result<Assignment>.Failure(ResultError.NotFound,"Assignment not found");
        }
        return Result<Assignment>.Success(assignment);
    }
    
    async Task<Result<Guid>> IAssignmentService.Create(string displayName, string? description, int duration, Guid? assigneeGuid, Guid? requiredRoleGuid, Guid? parentObjectiveGuid)
    {
        if (String.IsNullOrEmpty(displayName))
        {
            return Result<Guid>.Failure(ResultError.ValidationError,"Display name is required");
        }

        Assignment? existingAssignment = await assignmentRepository.QueryAssignment(displayName);
        if (existingAssignment != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict,"Assignment with this name already exists");
        }

        if (assigneeGuid.HasValue)
        {
            Actor? assignee = await actorRepository.QueryActor(assigneeGuid.Value);
            if (assignee == null)
            {
                return Result<Guid>.Failure(ResultError.NotFound, "The actor with this guid doesn't exists");
            }
        }

        if (requiredRoleGuid.HasValue)
        {
            Role? requiredRole = await roleRepository.QueryRole(requiredRoleGuid.Value);
            if (requiredRole == null)
            {
                return Result<Guid>.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
            }
        }

        if (assigneeGuid.HasValue && requiredRoleGuid.HasValue)
        {
            if (await DoesAssigneeHaveRequiredRole(assigneeGuid, requiredRoleGuid) == false)
            {
                return Result<Guid>.Failure(ResultError.ValidationError,"Actor does not have a required role");
            }
        }

        if (duration < 0)
        {
            return Result<Guid>.Failure(ResultError.ValidationError,"Duration cannot be negative");
        }

        Priority assignmentPriority = Priority.MidTerm;
        if (parentObjectiveGuid.HasValue)
        {
            Objective? parentObjective = await objectiveRepository.QueryObjective(parentObjectiveGuid.Value);
            if (parentObjective == null)
            {
                return Result<Guid>.Failure(ResultError.NotFound, "The objective with this guid doesn't exists");
            }
            assignmentPriority = CalculateAssignmentPriority(parentObjective.DeadlineDate, duration);
        }

        var maxSequenceNumber = (await assignmentRepository.GetMaxSequenceNumber() ?? 0) + 1;
        Assignment assignment = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = displayName,
            Description = description,
            Duration = duration,
            SequenceNumber = maxSequenceNumber,
            AssigneeGuid = assigneeGuid,
            RequiredRoleGuid = requiredRoleGuid,
            Priority = assignmentPriority,
            Status = AssignmentStatus.Planned,
            ParentObjectiveGuid = parentObjectiveGuid
        };
        
        await assignmentRepository.InsertAssignment(assignment);
        return Result<Guid>.Success(assignment.Guid);
    }

    async Task<Result> IAssignmentService.SetDisplayName(Guid guid, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result.Failure(ResultError.ValidationError, "The display name is required");
        }
        
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        Assignment? duplicate  = await assignmentRepository.QueryAssignment(displayName);
        if (duplicate != null)
        {
            return Result.Failure(ResultError.Conflict,"Assignment with this name already exists");
        }
        
        await assignmentRepository.UpdateDisplayName(guid, displayName);
        return Result.Success();
    }

    async Task<Result> IAssignmentService.SetDescription(Guid guid, string? description)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        await assignmentRepository.UpdateDescription(guid, description);
        return Result.Success();
    }

    async Task<Result> IAssignmentService.SetDuration(Guid guid, int duration)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }

        if (duration < 0)
        {
            return Result.Failure(ResultError.ValidationError,"Duration cannot be negative");
        }

        if (assignment.ParentObjectiveGuid.HasValue)
        {
            Objective? parentObjective = await objectiveRepository.QueryObjective(assignment.ParentObjectiveGuid.Value);
            if (parentObjective != null)
            {
                Priority assignmentPriority = CalculateAssignmentPriority(parentObjective.DeadlineDate, duration);
                await assignmentRepository.UpdatePriority(guid, assignmentPriority);
            }
        }
        
        await assignmentRepository.UpdateDuration(guid, duration);
        return Result.Success();
    }
    
    async Task<Result> IAssignmentService.SetAssignee(Guid guid, Guid? assigneeGuid)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }

        if (assigneeGuid != null)
        {
            Actor? actor = await actorRepository.QueryActor(assigneeGuid.Value);
            if (actor == null)
            {
                return Result.Failure(ResultError.NotFound,"Actor with this guid does not exist");
            }

            if (assignment.RequiredRoleGuid.HasValue)
            {
                if (await DoesAssigneeHaveRequiredRole(assigneeGuid, assignment.RequiredRoleGuid.Value) == false)
                {
                    return Result.Failure(ResultError.ValidationError,"Actor does not have a required role");
                }
            }
        }
        
        await assignmentRepository.UpdateAssignee(guid, assigneeGuid);
        return Result.Success();
    }

    async Task<Result> IAssignmentService.SetRequiredRole(Guid guid, Guid? requiredRoleGuid)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }

        if (requiredRoleGuid != null)
        {
            Role? role = await roleRepository.QueryRole(requiredRoleGuid.Value);
            if (role == null)
            {
                return Result.Failure(ResultError.NotFound,"Role with this guid does not exist");
            }
            
            if (assignment.AssigneeGuid.HasValue)
            {
                if (await DoesAssigneeHaveRequiredRole(assignment.AssigneeGuid.Value, requiredRoleGuid) == false)
                {
                    return Result.Failure(ResultError.ValidationError,"Actor does not have a required role");
                }
            }
        }
        await assignmentRepository.UpdateRequiredRole(guid, requiredRoleGuid);
        return Result.Success();
    }
    
    async Task<Result> IAssignmentService.SetPriority(Guid guid, Priority priority)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        await assignmentRepository.UpdatePriority(guid, priority);
        return Result.Success();
    }

    async Task<Result> IAssignmentService.SetStatus(Guid guid, AssignmentStatus assignmentStatus)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }

        if (assignmentStatus == AssignmentStatus.Completed)
        {
           Guid? nextAssignmentGuid = await assignmentRepository.GetNextAssignmentGuid(assignment.SequenceNumber);
           if (nextAssignmentGuid.HasValue)
            {
                Assignment? nextAssignment = await assignmentRepository.QueryAssignment(nextAssignmentGuid.Value);
                if (nextAssignment!.Status == AssignmentStatus.Planned)
                {
                    await assignmentRepository.UpdateStatus(nextAssignmentGuid.Value, AssignmentStatus.InProgress);
                }
            }
        }
        
        await assignmentRepository.UpdateStatus(guid, assignmentStatus);
        return Result.Success();
    }
    
    async Task<Result> IAssignmentService.SetParentObjective(Guid guid, Guid? parentObjectiveGuid)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }

        if (parentObjectiveGuid != null)
        {
            Objective? objective = await objectiveRepository.QueryObjective(parentObjectiveGuid.Value);
            if (objective == null)
            {
                return Result.Failure(ResultError.NotFound,"Objective with this guid does not exist");
            }

            Priority assignmentPriority = CalculateAssignmentPriority(objective.DeadlineDate, assignment.Duration);
            await assignmentRepository.UpdatePriority(guid, assignmentPriority);
        }

        await assignmentRepository.UpdateParentObjective(guid, parentObjectiveGuid);
        return Result.Success();
    }
    
    async Task<Result> IAssignmentService.Delete(Guid guid)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        await assignmentRepository.DeleteAssignment(guid);
        return Result.Success();
    }
}
