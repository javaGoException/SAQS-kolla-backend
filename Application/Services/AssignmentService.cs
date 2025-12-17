using SAQS_kolla_backend.Application.Common;
using SAQS_kolla_backend.Application.Interfaces;
using SAQS_kolla_backend.Domain.Enums;
using SAQS_kolla_backend.Domain.ValueObjects;

namespace SAQS_kolla_backend.Application.Services;

public class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
{
    async Task<Result<Guid>> IAssignmentService.Create(string DisplayName, string? Description, DateTimeOffset? StartDate, DateTimeOffset? DeadlineDate, Guid? AssigneeGuid, Guid? RequiredRole)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.Delete(Guid guid)
    {
        throw new NotImplementedException();
    }

    async Task<Result<Assignment>> IAssignmentService.Get(Guid guid)
    {
        throw new NotImplementedException();
    }

    async Task<Result<List<Guid>>> IAssignmentService.GetAllGuids()
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetAssignee(Guid Guid, Guid? AssigneeGuid)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetDeadlineDate(Guid Guid, DateTimeOffset? DeadlineDate)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetDescription(Guid Guid, string? Description)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetDisplayName(Guid Guid, string DisplayName)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetPriority(Guid Guid, Priority Priority)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetStartDate(Guid Guid, DateTimeOffset? StartDate)
    {
        throw new NotImplementedException();
    }

    async Task<Result> IAssignmentService.SetStatus(Guid Guid, AssignmentStatus AssignmentStatus)
    {
        throw new NotImplementedException();
    }
}