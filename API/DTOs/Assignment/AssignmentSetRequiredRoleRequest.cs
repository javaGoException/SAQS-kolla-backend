using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetRequiredRoleRequest
(
    [Required] Guid Guid,
    [Required] Guid? RequiredRoleGuid
);