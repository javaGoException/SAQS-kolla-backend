using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Application.Services;

namespace API;

public static class ObjectiveEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/CreateObjective", async ([FromBody] CreateObjectiveRequest? createObjectiveRequest, IObjectiveService objectiveService) =>
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

            return Results.Ok(new CreateObjectiveResponse(createObjectiveDTO.Guid));
        });

        
    }
}