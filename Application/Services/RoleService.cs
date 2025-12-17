using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    async Task<Result<List<Guid>>> IRoleService.GetAllGuids()
    {
        List<Guid> guids = await roleRepository.QueryAllRolesGuids();
        return Result<List<Guid>>.Success(guids);
    }

    async Task<Result<Role>> IRoleService.Get(Guid guid)
    {
        Role? role = await roleRepository.QueryRole(guid);

        if (role == null)
        {
            return Result<Role>.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
        }

        return Result<Role>.Success(role);
    }

    async Task<Result<Guid>> IRoleService.Create(string name, string? description, bool isAdmin)
    {
        Role? existingRole = await roleRepository.QueryRole(name);

        if (existingRole != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict, "The role with this name already exists");
        }

        Role role = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = name,
            Description = description,
            IsAdmin = isAdmin
        };
        await roleRepository.InsertRole(role);

        return Result<Guid>.Success(role.Guid);
    }

    async Task<Result> IRoleService.SetDisplayName(Guid guid, string displayName)
    {
        Role? role = await roleRepository.QueryRole(guid);

        if (role == null)
        {
            return Result.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
        }

        await roleRepository.UpdateDisplayName(guid, displayName);
        return Result.Success();
    }

    async Task<Result> IRoleService.SetDescription(Guid guid, string? description)
    {
        Role? role = await roleRepository.QueryRole(guid);

        if (role == null)
        {
            return Result.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
        }

        await roleRepository.UpdateDescription(guid, description);
        return Result.Success();
    }

    async Task<Result> IRoleService.SetAdminFlag(Guid guid, bool isAdmin)
    {
        Role? role = await roleRepository.QueryRole(guid);

        if (role == null)
        {
            return Result.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
        }

        await roleRepository.UpdateAdminFlag(guid, isAdmin);
        return Result.Success();
    }

    async Task<Result> IRoleService.Delete(Guid guid)
    {
        Role? role = await roleRepository.QueryRole(guid);

        if (role == null)
        {
            return Result.Failure(ResultError.NotFound, "The role with this guid doesn't exists");
        }

        await roleRepository.DeleteRole(guid);
        return Result.Success();
    }
}