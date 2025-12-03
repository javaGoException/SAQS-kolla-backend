using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs;
using SAQS_kolla_backend.Application.Services;
using SAQS_kolla_backend.Infrastructure.Services;
using SAQS_kolla_backend.Infrastructure.DTOs;

namespace SAQS_kolla_backend.API;

public static class ObjectiveEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("Objectives/GetObjective/{guid}", async (string? guid, IDatabaseManager databaseManager) =>
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

            ObjectiveDto? objective = await databaseManager.QueryObjective(Guid.Parse(guid));
            if(objective == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(objective);
        });

        app.MapGet("Objectives/GetAllObjectivesGuids",async (IDatabaseManager databaseManager) =>
        {
            List<Guid> guids = await databaseManager.QueryAllObjectivesGuids();
            return Results.Ok(guids);
        });

        app.MapPost("Objectives/CreateObjective", async ([FromBody] CreateObjectiveRequest? createObjectiveRequest, IObjectiveService objectiveService) =>
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

            var createObjectiveDTO = await objectiveService.CreateObjective(createObjectiveRequest.Name, createObjectiveRequest.Description);

            if(string.IsNullOrEmpty(createObjectiveDTO.Error) == false)
            {
                return Results.BadRequest(new {error = createObjectiveDTO.Error});
            }

            return Results.Ok(new {guid = createObjectiveDTO.Guid});
        });

        
    }
}