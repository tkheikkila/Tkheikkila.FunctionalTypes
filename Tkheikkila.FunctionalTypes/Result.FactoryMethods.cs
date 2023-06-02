namespace Tkheikkila.FunctionalTypes;

public static class Result
{
    public static Result<TValue, TError> Success<TValue, TError>(TValue value)
    {
        return new Result<TValue, TError>(value);
    }

    public static Result<TError> Success<TError>()
    {
        return new Result<TError>();
    }

    public static Task<Result<TValue, TError>> SuccessTask<TValue, TError>(TValue value)
    {
        return Task.FromResult(Success<TValue, TError>(value));
    }

    public static Task<Result<TError>> SuccessTask<TError>()
    {
        return Task.FromResult(Success<TError>());
    }

    public static Result<TValue, TError> Failure<TValue, TError>(TError error)
    {
        return new Result<TValue, TError>(error);
    }

    public static Result<TError> Failure<TError>(TError error)
    {
        return new Result<TError>(error);
    }

    public static Task<Result<TValue, TError>> FailureTask<TValue, TError>(TError error)
    {
        return Task.FromResult(Failure<TValue, TError>(error));
    }

    public static Task<Result<TError>> FailureTask<TError>(TError error)
    {
        return Task.FromResult(Failure(error));
    }
}
