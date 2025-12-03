namespace SAQS_kolla_backend.Infrastructure.DTOs;

public record ObjectiveDto(
    string Guid, 
    string DisplayName, 
    string? Description
);