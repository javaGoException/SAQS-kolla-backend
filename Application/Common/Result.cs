namespace SAQS_kolla_backend.Application.Common;

public abstract class ResultBase
{
    public bool IsSuccess {get;}
    public ResultError ResultError {get;} = ResultError.None;
    public string? Error {get;}
    
    protected ResultBase(bool isSuccess, ResultError resultError, string? error)
    {
        IsSuccess = isSuccess;
        ResultError = resultError;
        Error = error;
    }
}

public class Result : ResultBase
{
    private Result(bool isSuccess, ResultError resultError, string? error) : base(isSuccess, resultError, error){}
    public static Result Success() => new(true, ResultError.None, null);
    public static Result Failure(ResultError resultError, string error) => new(false, resultError, error);
}

public class Result<T> : ResultBase
{
    private readonly T _data;

    private Result(bool isSuccess, T data, ResultError resultError, string? error) : base(isSuccess, resultError, error)
    {
        _data = data;
    }
    public static Result<T> Success(T data) => new(true, data, ResultError.None, null);
    public static Result<T> Failure(ResultError resultError, string error) => new(false, default!, resultError, error);
    public T Data => IsSuccess ? _data : throw new InvalidOperationException($"Cannot access Value when IsSuccess is false. Error: {Error}");
}