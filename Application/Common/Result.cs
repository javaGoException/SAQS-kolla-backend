namespace SAQS_kolla_backend.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get;}
    public string? Error;
    private T _data;

    private Result(bool isSuccess, T data, string? error)
    {
        IsSuccess = isSuccess;
        _data = data;
        Error = error;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, default!, error);
    public T Data => IsSuccess ? _data : throw new InvalidOperationException($"Cannot access Value when IsSuccess is false. Error: {Error}");
}