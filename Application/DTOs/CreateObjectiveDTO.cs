namespace SAQS_kolla_backend.Application.DTOs;

public record CreateObjectiveDto(
    Guid Guid,
    string Error = ""
);