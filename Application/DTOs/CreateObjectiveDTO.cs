namespace Application.DTOs;

public record CreateObjectiveDto(
    Guid Guid,
    string Error = ""
);