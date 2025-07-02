namespace AgentState.Application.Shared;


// this can be abstract to a new project to be reused as a package
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public object? Error { get; }

    public bool IsFailure => !IsSuccess;

    private Result(bool isSuccess, T? value, object? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(object error) => new(false, default, error);
}