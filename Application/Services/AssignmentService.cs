using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class AssignmentService(IAssignmentRepository assignmentRepository, IActorRepository actorRepository, IRoleRepository roleRepository, IObjectiveRepository objectiveRepository) : IAssignmentService
{
    private Priority CalculatePriority(DateTimeOffset startDate, DateTimeOffset deadlineDate)
    {
        double hoursBetweenStartAndDeadline = (int)(deadlineDate - startDate).TotalHours;

        if (hoursBetweenStartAndDeadline <= 8)
        {
            return Priority.ShortTerm;
        } 
        
        if (hoursBetweenStartAndDeadline > 8 && hoursBetweenStartAndDeadline <= 32)
        {
            return Priority.MidTerm;
        }
        
        return Priority.LongTerm;
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
    
    async Task<Result<Guid>> IAssignmentService.Create(string displayName, string? description, DateTimeOffset? startDate, DateTimeOffset? deadlineDate, Guid? assigneeGuid, Guid? requiredRole)
    {
        if (String.IsNullOrEmpty(displayName))
        {
            return Result<Guid>.Failure(ResultError.ValidationError,"Display name is required");
        }

        Assignment? existingAssignment = await assignmentRepository.QueryAssignment(displayName);
        if (existingAssignment != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict,"Assignment already exists");
        }

        if (startDate != null)
        {
            if (startDate < DateTimeOffset.Now)
            {
                return Result<Guid>.Failure(ResultError.ValidationError, "StartDate cannot be in the past");
            }
        }

        if (deadlineDate != null)
        {
            if (deadlineDate < DateTimeOffset.Now)
            {
                return Result<Guid>.Failure(ResultError.ValidationError, "Deadline cannot be in the past");
            }
        }

        Priority assignmentPriority = Priority.MidTerm;
        if (startDate != null && deadlineDate != null)
        {
            if (deadlineDate <= startDate)
            {
                return Result<Guid>.Failure(ResultError.ValidationError, "Deadline cannot be before StartDate");
            }
            assignmentPriority = CalculatePriority(startDate.Value, deadlineDate.Value);
        }

        Assignment assignment = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = displayName,
            Description = description,
            StartDate = startDate,
            EndDate = null,
            DeadlineDate = deadlineDate,
            AssigneeGuid = assigneeGuid,
            RequiredRoleGuid = requiredRole,
            Priority = assignmentPriority,
            Status = AssignmentStatus.Planned
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
    
    async Task<Result> IAssignmentService.SetStartDate(Guid guid, DateTimeOffset? startDate)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        if (startDate != null && assignment.DeadlineDate != null)
        {
            Priority assignmentPriority = Priority.MidTerm;
            
            if (assignment.DeadlineDate <= startDate)
            {
                return Result.Failure(ResultError.ValidationError, "Cannot set StartDate after the Deadline");
            }
            
            assignmentPriority = CalculatePriority(startDate.Value, assignment.DeadlineDate.Value);
            await assignmentRepository.UpdatePriority(guid, assignmentPriority);
        }
        
        await assignmentRepository.UpdateStartDate(guid, startDate);
        return Result.Success();
    }

    async Task<Result> IAssignmentService.SetDeadlineDate(Guid guid, DateTimeOffset? deadlineDate)
    {
        Assignment? assignment = await assignmentRepository.QueryAssignment(guid);
        if (assignment == null)
        {
            return Result.Failure(ResultError.NotFound,"Assignment not found");
        }
        
        if (assignment.StartDate != null && deadlineDate != null)
        {
            Priority assignmentPriority = Priority.MidTerm;
            
            if (deadlineDate <= assignment.StartDate)
            {
                return Result.Failure(ResultError.ValidationError, "Cannot set Deadline before the StartDate");
            }
            
            assignmentPriority = CalculatePriority(assignment.StartDate.Value, deadlineDate.Value);
            await assignmentRepository.UpdatePriority(guid, assignmentPriority);
        }
        
        await assignmentRepository.UpdateDeadlineDate(guid, deadlineDate);
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
            await assignmentRepository.UpdateEndDate(guid, DateTimeOffset.Now);
        }
        else
        {
            await assignmentRepository.UpdateEndDate(guid, null);
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
