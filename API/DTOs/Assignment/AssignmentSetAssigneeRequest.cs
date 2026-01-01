using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetAssigneeRequest(
    [Required] Guid Guid,
    Guid? AssigneeGuid
);