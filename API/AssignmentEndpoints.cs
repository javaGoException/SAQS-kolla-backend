using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SAQS_kolla_backend.API.DTOs.Assignment;
using SAQS_kolla_backend.API.Hubs;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.API;

public static class AssignmentEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("Assignment/GetAll",async (IAssignmentService assignmentService) =>
        {
            Result<List<Guid>> result = await assignmentService.GetAllGuids();
            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapGet("Assignment/Get/{guid}", async (Guid guid, IAssignmentService assignmentService) =>
        {
            Result<Assignment> result = await assignmentService.Get(guid);
            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Assignment/Create", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentCreateRequest assignmentCreateRequest, IAssignmentService assignmentService) =>
        {
            Result<Guid> result = await assignmentService.Create(
                assignmentCreateRequest.DisplayName,
                assignmentCreateRequest.Description,
                assignmentCreateRequest.Duration,
                assignmentCreateRequest.AssigneeGuid,
                assignmentCreateRequest.RequiredRoleGuid,
                assignmentCreateRequest.ParentObjectiveGuid
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", result.Data);
            return Results.Ok(new {guid = result.Data});
        });

        app.MapPatch("Assignment/SetDisplayName", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetDisplayNameRequest assignmentSetDisplayNameRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetDisplayName(
                assignmentSetDisplayNameRequest.Guid,
                assignmentSetDisplayNameRequest.DisplayName
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetDisplayNameRequest.Guid);
            return Results.NoContent();
        });
        
        app.MapPatch("Assignment/SetDescription", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetDescriptionRequest assignmentSetDescriptionRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetDescription(
                assignmentSetDescriptionRequest.Guid,
                assignmentSetDescriptionRequest.Description
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetDescriptionRequest.Guid);
            return Results.NoContent();
        });

        app.MapPatch("Assignment/SetDuration", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetDurationRequest assignmentSetDurationRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetDuration(
                assignmentSetDurationRequest.Guid,
                assignmentSetDurationRequest.Duration
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetDurationRequest.Guid);
            return Results.NoContent();
        });

        app.MapPatch("Assignment/SetAssignee", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetAssigneeRequest assignmentSetAssigneeRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetAssignee(
                assignmentSetAssigneeRequest.Guid,
                assignmentSetAssigneeRequest.AssigneeGuid
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetAssigneeRequest.Guid);
            return Results.NoContent();
        });
        
        app.MapPatch("Assignment/SetRequiredRole", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetRequiredRoleRequest assignmentSetRequiredRole, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetRequiredRole(
                assignmentSetRequiredRole.Guid,
                assignmentSetRequiredRole.RequiredRoleGuid
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }
            
            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetRequiredRole.Guid);
            return Results.NoContent();
        });
        
        app.MapPatch("Assignment/SetPriority", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetPriorityRequest assignmentSetPriorityRequest, IAssignmentService assignmentService) =>
        {
            if (Enum.IsDefined(typeof(Priority), assignmentSetPriorityRequest.Priority) == false)
            {
                return Results.BadRequest(new {error = "Priority is invalid"});
            }

            Result result = await assignmentService.SetPriority(
                assignmentSetPriorityRequest.Guid,
                (Priority)assignmentSetPriorityRequest.Priority
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetPriorityRequest.Guid);
            return Results.NoContent();
        });

        app.MapPatch("Assignment/SetStatus", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetStatusRequest assignmentSetStatusRequest, IAssignmentService assignmentService) =>
        {
            if (Enum.IsDefined(typeof(AssignmentStatus), assignmentSetStatusRequest.AssignmentStatus) == false)
            {
                return Results.BadRequest(new {error = "AssignmentStatus is invalid"});
            }

            Result result = await assignmentService.SetStatus(
                assignmentSetStatusRequest.Guid,
                (AssignmentStatus)assignmentSetStatusRequest.AssignmentStatus
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetStatusRequest.Guid);
            return Results.NoContent();
        });
        
        app.MapPatch("Assignment/SetParentObjective", async (IHubContext<AssignmentHub> hubContext, [FromBody] AssignmentSetParentObjectiveRequest assignmentSetParentObjectiveRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetParentObjective(
                assignmentSetParentObjectiveRequest.Guid,
                assignmentSetParentObjectiveRequest.ParentObjectiveGuid
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", assignmentSetParentObjectiveRequest.Guid);
            return Results.NoContent();
        });

        app.MapDelete("Assignment/Delete/{guid}", async (IHubContext<AssignmentHub> hubContext, Guid guid, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.Delete(guid);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            await hubContext.Clients.All.SendAsync("OnAssignmentUpdated", guid);
            return Results.NoContent();
        });
    }
}