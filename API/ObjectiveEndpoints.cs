using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs;
using SAQS_kolla_backend.Application;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.API;

public static class ObjectiveEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("Objective/GetAll",async (IObjectiveService objectiveService) =>
        {
            Result<List<Guid>> result = await objectiveService.GetAllGuids();
            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapGet("Objective/Get/{guid}", async (string? guid, ValidatorService validatorService, IObjectiveService objectiveService) =>
        {
            if (validatorService.isGuidValid(guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result<Objective> result = await objectiveService.Get(Guid.Parse(guid!));
            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Objective/Create", async ([FromBody] ObjectiveCreateRequest? objectiveCreateRequest, IObjectiveService objectiveService) =>
        {
            if (objectiveCreateRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (string.IsNullOrEmpty(objectiveCreateRequest.Name))
            {
                return Results.BadRequest(new {error = "Objective name is required"});
            }

            Result<Guid> result = await objectiveService.Create(objectiveCreateRequest.Name, objectiveCreateRequest.Description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new {guid = result.Data});
        });

        app.MapPatch("Objective/SetObjectiveDescription", async (string? guid, string? description, ValidatorService validatorService, IObjectiveService objectiveService) =>
        {
            if(validatorService.isGuidValid(guid)==false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result result = await objectiveService.SetObjectiveDescription(Guid.Parse(guid!), description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapDelete("Objective/Delete/{guid}", async (string? guid, ValidatorService validatorService, IObjectiveService objectiveService) =>
        {
            if (validatorService.isGuidValid(guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result result = await objectiveService.Delete(Guid.Parse(guid!));

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });
    }
}