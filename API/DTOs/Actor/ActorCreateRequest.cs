using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorCreateRequest(
    [Required] string DisplayName,
    Guid? RoleGuid
);
