using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> QueryRole(Guid guid);
    Task<Role?> QueryRole(string name);
    Task<List<Guid>> QueryAllRolesGuids();
    Task<bool> InsertRole(Role role);
    Task<bool> UpdateDisplayName(Guid guid, string displayName);
    Task<bool> UpdateDescription(Guid guid, string? description);
    Task<bool> UpdateAdminFlag(Guid guid, bool isAdmin);
    Task<bool> DeleteRole(Guid guid);
}