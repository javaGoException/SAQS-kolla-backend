using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IActorService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Actor>> Get(Guid guid);
    Task<Result<Guid>> Create(string nickname, Guid? roleGuid);
    Task<Result> SetNickname(Guid guid, string nickname);
    Task<Result> SetRole(Guid guid, Guid? roleGuid);
}
