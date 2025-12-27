using SAQS_kolla_backend.Application.Common;

namespace SAQS_kolla_backend.API;

public static class ErrorMapper
{
    public static IResult Map(ResultError resultError, string error)
    {
        return resultError switch
        {
            ResultError.None => Results.Ok(),
            ResultError.NotFound => Results.NotFound(new {error}),
            ResultError.ValidationError => Results.BadRequest(new {error}),
            ResultError.Conflict => Results.Conflict(new {error}),
            ResultError.Unauthorized => Results.Unauthorized(),
            _ => Results.Problem(statusCode: 500, detail:"An unexpected error occurred.")
        };
    }
}
