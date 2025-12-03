using SAQS_kolla_backend.Application.DTOs;

namespace SAQS_kolla_backend.Application.Services;

public interface IObjectiveService
{
    Task<CreateObjectiveDto> CreateObjective(string name, string description);
}