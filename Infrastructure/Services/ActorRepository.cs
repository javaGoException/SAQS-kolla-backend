using Dapper;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Infrastructure.DTOs;
using SAQS_kolla_backend.Infrastructure.Setup;

namespace SAQS_kolla_backend.Infrastructure.Services;

public class ActorRepository(IDatabaseConnector databaseConnector) : IActorRepository
{
    public async Task<Actor?> QueryActor(Guid guid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        
        // Wir machen einen JOIN, um die Role-Daten direkt mitzuladen
        string sql = @"
            SELECT 
                a.Guid, a.DisplayName, a.RoleGuid,
                r.Guid, r.DisplayName, r.Description, r.IsAdmin
            FROM Actors a
            LEFT JOIN Roles r ON a.RoleGuid = r.Guid
            WHERE a.Guid = @Guid;";

        // Wir nutzen Dappers Multi-Mapping Feature
        var result = await connection.QueryAsync<ActorDto, RoleDto?, Actor>(
            sql,
            (actorDto, roleDto) =>
            {
                var actor = new Actor
                {
                    Guid = Guid.Parse(actorDto.Guid),
                    DisplayName = actorDto.DisplayName,
                    Role = roleDto == null ? null : new Role
                    {
                        Guid = Guid.Parse(roleDto.Guid),
                        DisplayName = roleDto.DisplayName,
                        Description = roleDto.Description,
                        IsAdmin = roleDto.IsAdmin
                    }
                };
                return actor;
            },
            new { Guid = guid },
            splitOn: "Guid" // Dapper Splitter, da beide Tabellen 'Guid' als erste Spalte haben
        );

        return result.SingleOrDefault();
    }

    public async Task<Actor?> QueryActorByNickname(string nickname)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        
        string sql = @"
            SELECT 
                a.Guid, a.DisplayName, a.RoleGuid,
                r.Guid, r.DisplayName, r.Description, r.IsAdmin
            FROM Actors a
            LEFT JOIN Roles r ON a.RoleGuid = r.Guid
            WHERE a.DisplayName = @Nickname;";

        var result = await connection.QueryAsync<ActorDto, RoleDto?, Actor>(
            sql,
            (actorDto, roleDto) =>
            {
                return new Actor
                {
                    Guid = Guid.Parse(actorDto.Guid),
                    DisplayName = actorDto.DisplayName,
                    Role = roleDto == null ? null : new Role
                    {
                        Guid = Guid.Parse(roleDto.Guid),
                        DisplayName = roleDto.DisplayName,
                        Description = roleDto.Description,
                        IsAdmin = roleDto.IsAdmin
                    }
                };
            },
            new { Nickname = nickname },
            splitOn: "Guid"
        );

        return result.SingleOrDefault();
    }

    public async Task<List<Guid>> QueryAllActorGuids()
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "SELECT Guid FROM Actors;";

        IEnumerable<string> stringGuids = await connection.QueryAsync<string>(sql);
        return stringGuids.Select(g => Guid.Parse(g)).ToList();
    }

    public async Task<bool> InsertActor(Actor actor, Guid? roleGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "INSERT INTO Actors(Guid, DisplayName, RoleGuid) VALUES (@Guid, @DisplayName, @RoleGuid);";

        var affectedRows = await connection.ExecuteAsync(sql, new { 
            Guid = actor.Guid, 
            DisplayName = actor.DisplayName, 
            RoleGuid = roleGuid 
        });
        return affectedRows > 0;
    }

    public async Task<bool> UpdateNickname(Guid guid, string nickname)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Actors SET DisplayName = @DisplayName WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, DisplayName = nickname });
        return affectedRows > 0;
    }

    public async Task<bool> UpdateRole(Guid guid, Guid? roleGuid)
    {
        using var connection = await databaseConnector.OpenConnectionAsync();
        string sql = "UPDATE Actors SET RoleGuid = @RoleGuid WHERE Guid = @Guid;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Guid = guid, RoleGuid = roleGuid });
        return affectedRows > 0;
    }
}
