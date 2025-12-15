using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Application.Common;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveService
{
    Task<Result<List<Guid>>> GetAllGuids();
    Task<Result<Objective>> Get(Guid guid);
    Task<Result<Guid>> Create(string name, string? description);
    Task<Result> SetObjectiveDescription(Guid guid, string? description);
    Task<Result> Delete(Guid guid);
}