using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Application.Common;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IRoleService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Role>> Get(Guid guid);
    Task<Result<Guid>> Create(string displayName, string? description);
    Task<Result> SetDisplayName(Guid guid, string displayName);
    Task<Result> SetDescription(Guid guid, string? description);
    Task<Result> SetAdminFlag(Guid guid, bool isAdmin);
    Task<Result> Delete(Guid guid);
}