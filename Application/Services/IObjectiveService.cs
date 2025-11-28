using Application.DTOs;

namespace Application.Services;

public interface IObjectiveService
{
    Task<CreateObjectiveDTO> CreateObjective(string name, string description);
}