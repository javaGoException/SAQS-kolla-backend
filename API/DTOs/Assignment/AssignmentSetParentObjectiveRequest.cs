using System.ComponentModel.DataAnnotations;

namespace SAQS_kolla_backend.API.DTOs.Assignment;

public record AssignmentSetParentObjectiveRequest
(
    [Required] Guid Guid,
    Guid? ParentObjectiveGuid
);