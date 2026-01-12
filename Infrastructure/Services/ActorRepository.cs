using Dapper;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Infrastructure.Setup;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class ActorRepository(IDatabaseConnector databaseConnector, IRoleRepository roleRepository) : IActorRepository
{
    async Task<Actor?> IActorRepository.QueryActor(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Actors a WHERE a.Guid = @Guid;";

        ActorDto? actorDto = await connection.QuerySingleOrDefaultAsync<ActorDto>(sql, new {Guid = guid});

        if (actorDto == null)
        {
            return null;
        }

        Role? role = null;
        if (actorDto.RoleGuid != null)
        {
            role = await roleRepository.QueryRole(Guid.Parse(actorDto.RoleGuid)); 
        }

        Actor actor = new()
        {
            Guid = Guid.Parse(actorDto.Guid),
            DisplayName = actorDto.DisplayName,
            Role = role,
            TenantId = actorDto.TenantId != null ? Guid.Parse(actorDto.TenantId) : null
        };
        return actor;
    }

    async Task<Actor?> IActorRepository.QueryActor(string displayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT * FROM Actors a WHERE a.DisplayName = @DisplayName;";

        ActorDto? actorDto = await connection.QuerySingleOrDefaultAsync<ActorDto>(sql, new {DisplayName = displayName});

        if (actorDto == null)
        {
            return null;
        }

        Role? role = null;
        if (actorDto.RoleGuid != null)
        {
            role = await roleRepository.QueryRole(actorDto.RoleGuid); 
        }

        Actor actor = new()
        {
            Guid = Guid.Parse(actorDto.Guid),
            DisplayName = actorDto.DisplayName,
            Role = role,
            TenantId = actorDto.TenantId != null ? Guid.Parse(actorDto.TenantId) : null
        };
        return actor;
    }

    async Task<List<Guid>> IActorRepository.QueryAllActorGuids(Guid? tenantId)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Actors";
        
        if(tenantId.HasValue)
        {
            sql += " WHERE TenantId = @TenantId";
        }
        
        sql += ";";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql, new { TenantId = tenantId });
        return stringGuids.Select(g => Guid.Parse(g)).ToList();
    }

    async Task<bool> IActorRepository.InsertActor(Actor actor, Guid? roleGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Actors(Guid, DisplayName, RoleGuid, TenantId) VALUES (@Guid, @DisplayName, @RoleGuid, @TenantId);";

        var affectedRows = await connection.ExecuteAsync(sql, new { 
            Guid = actor.Guid, 
            DisplayName = actor.DisplayName, 
            RoleGuid = roleGuid,
            TenantId = actor.TenantId
        });
        return affectedRows > 0;
    }

    async Task<bool> IActorRepository.UpdateDisplayName(Guid guid, string DisplayName)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Actors SET DisplayName = @DisplayName WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, DisplayName = DisplayName });
        return affectedRows > 0;
    }

    async Task<bool> IActorRepository.UpdateRole(Guid guid, Guid? roleGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Actors SET RoleGuid = @RoleGuid WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, RoleGuid = roleGuid });
        return affectedRows > 0;
    }

    async Task<List<Guid>> IActorRepository.QueryAllAssignments(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Assignments WHERE AssigneeGuid = @AssigneeGuid;";
        
        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql, new { AssigneeGuid = guid });
        return stringGuids.Select(g => Guid.Parse(g)).ToList();
    }
    
    async Task<bool> IActorRepository.Delete(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        
        string assignmentsSql = "UPDATE Assignments SET AssigneeGuid = NULL WHERE AssigneeGuid = @Guid;";
        var assignmentsAffectedRows = await connection.ExecuteAsync(assignmentsSql, new {Guid = guid});
        
        string actorSql = "DELETE FROM Actors WHERE Guid = @Guid;";
        var actorAffectedRows = await connection.ExecuteAsync(actorSql, new {Guid = guid});
        
        return assignmentsAffectedRows > 0 && actorAffectedRows > 0;
    }
}
