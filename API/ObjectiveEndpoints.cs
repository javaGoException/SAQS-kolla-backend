using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs;
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
            Result<List<Guid>> result = await objectiveService.GetAllObjectivesGuid();
            if (result.IsSuccess == false)
            {
                return Results.BadRequest(new {result.Error});
            }

            return Results.Ok(result.Data);
        });

        app.MapGet("Objective/Get/{guid}", async (IObjectiveService objectiveService, string? guid) =>
        {
            if (string.IsNullOrEmpty(guid))
            {
                return Results.NotFound();
            }

            try
            {
                Guid.Parse(guid);
            } 
            catch (FormatException)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result<Objective> result = await objectiveService.GetObjective(Guid.Parse(guid));
            if(result.IsSuccess == false)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Objective/Create", async ([FromBody] CreateObjectiveRequest? createObjectiveRequest, IObjectiveService objectiveService) =>
        {
            if (createObjectiveRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (string.IsNullOrEmpty(createObjectiveRequest.Name))
            {
                return Results.BadRequest(new {error = "Objective name is required"});
            }

            if (string.IsNullOrEmpty(createObjectiveRequest.Description))
            {
                return Results.BadRequest(new {error = "Objective description is required"});
            }

            Result<Guid> result = await objectiveService.CreateObjective(createObjectiveRequest.Name, createObjectiveRequest.Description);

            if(result.IsSuccess == false)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new {guid = result.Data});
        });

        
    }
}