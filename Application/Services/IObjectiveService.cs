using Application.DTOs;

namespace Application.Services;

public interface IObjectiveService
{
    Task<CreateObjectiveDto> CreateObjective(string name, string description);
}