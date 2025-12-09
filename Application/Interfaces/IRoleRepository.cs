using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> QueryRole(Guid guid);
    Task<Role?> QueryRole(string name);
    Task<List<Guid>> QueryAllRoleGuids();
    Task InsertRole(Role role);
    Task DeleteRole(Guid guid);
}