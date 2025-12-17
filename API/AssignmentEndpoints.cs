using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs.Assignment;
using SAQS_kolla_backend.Application;
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

        app.MapPost("Assignment/Create", async ([FromBody] AssignmentCreateRequest assignmentCreateRequest, IAssignmentService assignmentService) =>
        {
            if (string.IsNullOrEmpty(assignmentCreateRequest.DisplayName))
            {
                return Results.BadRequest(new {error = "DisplayName is required"});
            }

            Result<Guid> result = await assignmentService.Create(
                assignmentCreateRequest.DisplayName,
                assignmentCreateRequest.Description,
                assignmentCreateRequest.StartDate,
                assignmentCreateRequest.DeadlineDate,
                assignmentCreateRequest.AssigneeGuid,
                assignmentCreateRequest.RequiredRoleGuid
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new {guid = result.Data});
        });

        app.MapPost("Assignment/SetDisplayName", async ([FromBody] AssignmentSetDisplayNameRequest assignmentSetDisplayNameRequest, IAssignmentService assignmentService) =>
        {
            if (string.IsNullOrEmpty(assignmentSetDisplayNameRequest.DisplayName))
            {
                return Results.BadRequest(new {error = "DisplayName is required"});
            }

            Result result = await assignmentService.SetDisplayName(
                assignmentSetDisplayNameRequest.Guid,
                assignmentSetDisplayNameRequest.DisplayName
            );

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPost("Assignment/SetStartDate", async ([FromBody] AssignmentSetStartDateRequest assignmentSetStartDateRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetStartDate(
                assignmentSetStartDateRequest.Guid,
                assignmentSetStartDateRequest.StartDate
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPost("Assignment/SetDeadlineDate", async ([FromBody] AssignmentSetDeadlineDateRequest assignmentSetDeadlineDateRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetDeadlineDate(
                assignmentSetDeadlineDateRequest.Guid,
                assignmentSetDeadlineDateRequest.DeadlineDate
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPost("Assignment/SetAssignee", async ([FromBody] AssignmentSetAssigneeRequest assignmentSetAssigneeRequest, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.SetAssignee(
                assignmentSetAssigneeRequest.Guid,
                assignmentSetAssigneeRequest.AssigneeGuid
            );

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPost("Assignment/SetPriority", async ([FromBody] AssignmentSetPriorityRequest assignmentSetPriorityRequest, IAssignmentService assignmentService) =>
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

            return Results.NoContent();
        });

        app.MapPost("Assignment/SetStatus", async ([FromBody] AssignmentSetStatusRequest assignmentSetStatusRequest, IAssignmentService assignmentService) =>
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

            return Results.NoContent();
        });

        app.MapDelete("Assignment/Delete/{guid}", async (Guid guid, IAssignmentService assignmentService) =>
        {
            Result result = await assignmentService.Delete(guid);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });
    }
}