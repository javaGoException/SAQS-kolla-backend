using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class ActorService(IActorRepository actorRepository, IRoleRepository roleRepository) : IActorService
{
    public async Task<Result<List<Guid>>> GetAllGuids()
    {
        List<Guid> guids = await actorRepository.QueryAllActorGuids();
        return Result<List<Guid>>.Success(guids);
    }

    public async Task<Result<Actor>> Get(Guid guid)
    {
        Actor? actor = await actorRepository.QueryActor(guid);
        if (actor == null)
        {
            return Result<Actor>.Failure(ResultError.NotFound, "The actor with this guid doesn't exist");
        }
        return Result<Actor>.Success(actor);
    }

    public async Task<Result<Guid>> Create(string nickname, Guid? roleGuid)
    {
        // Check if nickname is taken
        Actor? existingActor = await actorRepository.QueryActorByNickname(nickname);
        if (existingActor != null)
        {
            return Result<Guid>.Failure(ResultError.Conflict, "The actor with this nickname already exists");
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
            DisplayName = nickname,
            Role = role
        };

        await actorRepository.InsertActor(actor, roleGuid);
        return Result<Guid>.Success(actor.Guid);
    }

    public async Task<Result> SetNickname(Guid guid, string nickname)
    {
        Actor? actor = await actorRepository.QueryActor(guid);
        if (actor == null)
        {
            return Result.Failure(ResultError.NotFound, "The actor with this guid doesn't exist");
        }

        // Check conflict if nickname changes
        if (actor.DisplayName != nickname)
        {
            Actor? duplicate = await actorRepository.QueryActorByNickname(nickname);
            if (duplicate != null)
            {
                return Result.Failure(ResultError.Conflict, "Nickname is already taken");
            }
        }

        await actorRepository.UpdateNickname(guid, nickname);
        return Result.Success();
    }

    public async Task<Result> SetRole(Guid guid, Guid? roleGuid)
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
