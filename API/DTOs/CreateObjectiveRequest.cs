namespace API.DTOs;

public record CreateObjectiveRequest(
    string Name,
    string Description
);