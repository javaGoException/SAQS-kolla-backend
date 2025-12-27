using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs.Actor;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.API;

public static class ActorEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("Actor/GetAll", async (IActorService actorService) =>
        {
            Result<List<Guid>> result = await actorService.GetAllGuids();
            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }
            return Results.Ok(result.Data);
        });

        app.MapGet("Actor/Get/{guid}", async (Guid guid, IActorService actorService) =>
        {
            Result<Actor> result = await actorService.Get(guid);
            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }
            return Results.Ok(result.Data);
        });

        app.MapPost("Actor/Create", async ([FromBody] ActorCreateRequest request, IActorService actorService) =>
        {
            Result<Guid> result = await actorService.Create(request.DisplayName, request.RoleGuid);

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new { guid = result.Data });
        });

        app.MapPatch("Actor/SetDisplayName", async ([FromBody] ActorSetDisplayNameRequest request, IActorService actorService) =>
        {
            Result result = await actorService.SetDisplayName(request.Guid, request.DisplayName);

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }
            return Results.NoContent();
        });

        app.MapPatch("Actor/SetRole", async ([FromBody] ActorSetRoleRequest request, IActorService actorService) =>
        {
            Result result = await actorService.SetRole(request.Guid, request.RoleGuid);

            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }
            return Results.NoContent();
        });
    }
}
