namespace SAQS_kolla_backend.API.DTOs;

public record CreateObjectiveRequest(
    string Name,
    string Description
);