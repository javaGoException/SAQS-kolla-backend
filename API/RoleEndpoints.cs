using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs.Role;
using SAQS_kolla_backend.Application;
using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.API;

public static class RoleEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("Role/GetAll",async (IRoleService roleService) =>
        {
            Result<List<Guid>> result = await roleService.GetAllGuids();
            if (result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapGet("Role/Get/{guid}", async (Guid guid, IRoleService roleService) =>
        {
            Result<Role> result = await roleService.Get(guid);
            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Role/Create", async ([FromBody] RoleCreateRequest roleCreateRequest, IRoleService roleService) =>
        {
            Result<Guid> result = await roleService.Create(roleCreateRequest.DisplayName, roleCreateRequest.Description, roleCreateRequest.isAdmin);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new {guid = result.Data});
        });

        app.MapPatch("Role/SetDisplayName", async ([FromBody] RoleSetDisplayNameRequest roleSetDisplayNameRequest, IRoleService roleService) =>
        {
            Result result = await roleService.SetDisplayName(roleSetDisplayNameRequest.Guid, roleSetDisplayNameRequest.DisplayName);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPatch("Role/SetDescription", async ([FromBody] RoleSetDescriptionRequest roleSetDescriptionRequest, IRoleService roleService) =>
        {
            Result result = await roleService.SetDescription(roleSetDescriptionRequest.Guid, roleSetDescriptionRequest.Description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPatch("Role/SetAdminFlag", async ([FromBody] RoleSetAdminFlagRequest roleSetAdminFlagRequest, IRoleService roleService) =>
        {
            Result result = await roleService.SetAdminFlag(roleSetAdminFlagRequest.Guid, roleSetAdminFlagRequest.isAdmin);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapDelete("Role/Delete/{guid}", async (Guid guid, IRoleService roleService) =>
        {
            Result result = await roleService.Delete(guid);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });
    }
}