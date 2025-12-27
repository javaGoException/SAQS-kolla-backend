using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IActorRepository
{
    Task<Actor?> QueryActor(Guid guid);
    Task<Actor?> QueryActor(string displayName);
    Task<List<Guid>> QueryAllActorGuids();
    Task<bool> InsertActor(Actor actor, Guid? roleGuid);
    Task<bool> UpdateDisplayName(Guid guid, string displayName);
    Task<bool> UpdateRole(Guid guid, Guid? roleGuid);
}
