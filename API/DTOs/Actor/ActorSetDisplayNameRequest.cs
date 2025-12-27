using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Actor;

public record ActorSetDisplayNameRequest(
    [Required] Guid Guid,
    [Required] string DisplayName
);
