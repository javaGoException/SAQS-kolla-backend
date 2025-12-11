using Dapper;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Application.Interfaces;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class RoleRepository(IDatabaseConnector databaseConnector) : IRoleRepository
{
    public async Task<Role?> QueryRole(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Roles r WHERE r.Guid = @Guid;";

        RoleDto? roleDto = await connection.QuerySingleOrDefaultAsync<RoleDto>(sql, new {Guid = guid});

        if (roleDto == null)
        {
            return null;
        }

        Role role = new()
        {
            Guid = Guid.Parse(roleDto.Guid),
            DisplayName = roleDto.DisplayName,
            Description = roleDto.Description,
            IsAdmin = roleDto.IsAdmin
        };
        return role;
    }

    public async Task<Role?> QueryRole(string name)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Roles r WHERE r.Name = @Name;";

        RoleDto? roleDto = await connection.QuerySingleOrDefaultAsync<RoleDto>(sql, new {Name = name});

        if (roleDto == null)
        {
            return null;
        }

        Role role = new()
        {
            Guid = Guid.Parse(roleDto.Guid),
            DisplayName = roleDto.DisplayName,
            Description = roleDto.Description,
            IsAdmin = roleDto.IsAdmin
        };
        return role;
    }

    public async Task<List<Guid>> QueryAllRolesGuids()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Roles;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        List<Guid> guids = stringGuids.Select(g => Guid.Parse(g)).ToList();
        return guids;
    }

    public async Task InsertRole(Role role)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Roles(Guid, DisplayName, Description, IsAdmin) VALUES (@Guid, @DisplayName, @Description, @IsAdmin);";

        await connection.ExecuteAsync(sql, role);
    }

    public async Task<bool> UpdateDisplayName(Guid guid, string displayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Roles SET DisplayName = @DisplayName WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, DisplayName = displayName});
        return affectedRows > 0;
    }

    public async Task<bool> UpdateDescription(Guid guid, string? description)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Roles SET Description = @Description WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, Description = description});
        return affectedRows > 0;
    }

    public async Task<bool> UpdateAdminFlag(Guid guid, bool isAdmin)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Roles SET IsAdmin = @IsAdmin WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid, IsAdmin = isAdmin});
        return affectedRows > 0;
    }

    public async Task<bool> DeleteRole(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "DELETE FROM Roles WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new {Guid = guid});
        return affectedRows > 0;
    }
}