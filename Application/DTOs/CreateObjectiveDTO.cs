namespace Application.DTOs;

public record CreateObjectiveDTO(
    Guid Guid,
    string Error = ""
);