using Microsoft.AspNetCore.Mvc;
using SAQS_kolla_backend.API.DTOs;
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

        app.MapGet("Role/Get/{guid}", async (ValidatorService validatorService, IRoleService roleService, string? guid) =>
        {
            if (validatorService.isGuidValid(guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result<Role> result = await roleService.Get(Guid.Parse(guid!));
            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("Role/Create", async ([FromBody] RoleCreateRequest? roleCreateRequest, IRoleService roleService) =>
        {
            if (roleCreateRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (string.IsNullOrEmpty(roleCreateRequest.Name))
            {
                return Results.BadRequest(new {error = "Role name is required"});
            }

            Result<Guid> result = await roleService.Create(roleCreateRequest.Name, roleCreateRequest.Description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.Ok(new {guid = result.Data});
        });

        app.MapPatch("Role/SetDisplayName", async ([FromBody] RoleSetDisplayNameRequest? roleSetDisplayNameRequest, ValidatorService validatorService, IRoleService roleService) =>
        {
            if (roleSetDisplayNameRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (validatorService.isGuidValid(roleSetDisplayNameRequest.Guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            if (string.IsNullOrEmpty(roleSetDisplayNameRequest.DisplayName))
            {
                return Results.BadRequest(new {error = "DisplayName is required"});
            }

            Result result = await roleService.SetDisplayName(Guid.Parse(roleSetDisplayNameRequest.Guid!), roleSetDisplayNameRequest.DisplayName);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPatch("Role/SetDescription", async ([FromBody] RoleSetDescriptionRequest? roleSetDescriptionRequest, ValidatorService validatorService, IRoleService roleService) =>
        {
            if (roleSetDescriptionRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (validatorService.isGuidValid(roleSetDescriptionRequest.Guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result result = await roleService.SetDescription(Guid.Parse(roleSetDescriptionRequest.Guid!), roleSetDescriptionRequest.Description);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapPatch("Role/SetAdminFlag", async ([FromBody] RoleSetAdminFlagRequest? roleSetAdminFlagRequest, ValidatorService validatorService, IRoleService roleService) =>
        {
            if (roleSetAdminFlagRequest == null)
            {
                return Results.BadRequest(new {error = "Request body is required"});
            }

            if (validatorService.isGuidValid(roleSetAdminFlagRequest.Guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result result = await roleService.SetAdminFlag(Guid.Parse(roleSetAdminFlagRequest.Guid!), roleSetAdminFlagRequest.isAdmin);

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });

        app.MapDelete("Role/Delete/{guid}", async (string? guid, ValidatorService validatorService, IRoleService roleService) =>
        {
            if (validatorService.isGuidValid(guid) == false)
            {
                return Results.BadRequest(new {error = "GUID is invalid"});
            }

            Result result = await roleService.Delete(Guid.Parse(guid!));

            if(result.IsSuccess == false)
            {
                return ErrorMapper.Map(result.ResultError, result.Error!);
            }

            return Results.NoContent();
        });
    }
}