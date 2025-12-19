using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs.Objective;
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

        app.MapGet("Objective/Get/{guid}", async (Guid guid, IObjectiveService objectiveService) =>
        {
            Result<Objective> result = await objectiveService.Get(guid);
            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Objective/Create", async ([FromBody] ObjectiveCreateRequest objectiveCreateRequest, IObjectiveService objectiveService) =>
        {
            if (string.IsNullOrEmpty(objectiveCreateRequest.DisplayName))
            {
                return Results.BadRequest(new {error = "DisplayName is required"});
            }

            Result<Guid> result = await objectiveService.Create(objectiveCreateRequest.DisplayName, objectiveCreateRequest.Description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new {guid = result.Data});
        });

        app.MapDelete("Objective/Delete/{guid}", async (Guid guid, IObjectiveService objectiveService) =>
        {
            Result result = await objectiveService.Delete(guid);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });
    }
}