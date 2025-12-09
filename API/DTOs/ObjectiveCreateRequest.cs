namespace SAQS_kolla_backend.API.DTOs;

public record ObjectiveCreateRequest(
    string? Name,
    string? Description
);