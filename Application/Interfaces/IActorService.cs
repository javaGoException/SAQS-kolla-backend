using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IActorService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Actor>> Get(Guid guid);
    Task<Result<Guid>> Create(string displayName, Guid? roleGuid);
    Task<Result> SetDisplayName(Guid guid, string displayName);
    Task<Result> SetRole(Guid guid, Guid? roleGuid);
}
