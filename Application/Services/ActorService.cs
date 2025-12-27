using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class ActorService(IActorRepository actorRepository, IRoleRepository roleRepository) : IActorService
{
    async Task<Result<List<Guid>>> IActorService.GetAllGuids()
    {
        List<Guid> guids = await actorRepository.QueryAllActorGuids();
        return Result<List<Guid>>.Success(guids);
    }

    async Task<Result<Actor>> IActorService.Get(Guid guid)
    {
        Actor? actor = await actorRepository.QueryActor(guid);
        if (actor == null)
        {
            return Result<Actor>.Failure(ResultError.NotFound, "The actor with this guid doesn't exist");
        }
        return Result<Actor>.Success(actor);
    }

    async Task<Result<Guid>> IActorService.Create(string displayName, Guid? roleGuid)
    {
        Actor? existingActor = await actorRepository.QueryActor(displayName);
        if (existingActor != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict, "The actor with this displayName already exists");
        }

        Role? role = null;
        if (roleGuid.HasValue)
        {
            role = await roleRepository.QueryRole(roleGuid.Value);
            if (role == null)
            {
                return Result<Guid>.Failure(ResultError.NotFound, "The specified role doesn't exist");
            }
        }

        Actor actor = new()
        {
            Guid = Guid.NewGuid(),
            DisplayName = displayName,
            Role = role
        };

        await actorRepository.InsertActor(actor, roleGuid);
        return Result<Guid>.Success(actor.Guid);
    }

    async Task<Result> IActorService.SetDisplayName(Guid guid, string displayName)
    {
        Actor? actor = await actorRepository.QueryActor(guid);
        if (actor == null)
        {
            return Result.Failure(ResultError.NotFound, "The actor with this guid doesn't exist");
        }
        
        Actor? duplicate = await actorRepository.QueryActor(displayName);
        if (duplicate != null)
        {
            return Result.Failure(ResultError.Conflict, "The actor with this name already exist");
        }
        
        await actorRepository.UpdateDisplayName(guid, displayName);
        return Result.Success();
    }

    async Task<Result> IActorService.SetRole(Guid guid, Guid? roleGuid)
    {
        Actor? actor = await actorRepository.QueryActor(guid);
        if (actor == null)
        {
            return Result.Failure(ResultError.NotFound, "The actor with this guid doesn't exist");
        }

        if (roleGuid.HasValue)
        {
            Role? role = await roleRepository.QueryRole(roleGuid.Value);
            if (role == null)
            {
                return Result.Failure(ResultError.NotFound, "The specified role doesn't exist");
            }
        }

        await actorRepository.UpdateRole(guid, roleGuid);
        return Result.Success();
    }
}
