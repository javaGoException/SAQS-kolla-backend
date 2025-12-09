using SAQS_kolla_backend.Domain.ValueObjects;
using SAQS_kolla_backend.Application.Common;

namespace SAQS_kolla_backend.Application.Interfaces;

public interface IObjectiveService
{
    Task<Result<List<Guid>>> GetAllObjectivesGuid();
    Task<Result<Objective>> GetObjective(Guid guid);
    Task<Result<Guid>> CreateObjective(string name, string description);
    Task<Result> DeleteObjective(Guid guid);
}