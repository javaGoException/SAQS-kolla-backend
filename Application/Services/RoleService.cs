using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class RoleService() : IRoleService
{
    public Task<Result<Guid>> Create(string displayName, string? description)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Delete(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Role>> Get(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Guid>>> GetAllGuids()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetAdminFlag(Guid guid, bool isAdmin)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetDescription(Guid guid, string? description)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetDisplayName(Guid guid, string displayName)
    {
        throw new NotImplementedException();
    }
}