using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IActorRepository
{
    Task<Actor?> QueryActor(Guid guid);
    Task<Actor?> QueryActorByNickname(string nickname);
    Task<List<Guid>> QueryAllActorGuids();
    Task<bool> InsertActor(Actor actor, Guid? roleGuid);
    Task<bool> UpdateNickname(Guid guid, string nickname);
    Task<bool> UpdateRole(Guid guid, Guid? roleGuid);
}
