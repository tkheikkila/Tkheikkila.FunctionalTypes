using System.Net.Http.Headers;

namespace Tkheikkila.FunctionalTypes;

public static class Result
{
    public static Result<TValue, TError> Success<TValue, TError>(TValue value)
    {
        return new Result<TValue, TError>(true, value, default!);
    }

    public static Result<TValue, Unit> Success<TValue>(TValue value)
    {
        return new Result<TValue, Unit>(true, value, default!);
    }

    public static Result<TError> Success<TError>()
    {
        return new Result<TError>(true, default!);
    }

    public static Result<Unit> Success()
    {
        return new Result<Unit>(true, Unit.Value);
    }

    public static Result<TValue, TError> Failure<TValue, TError>(TError error)
    {
        return new Result<TValue, TError>(false, default!, error);
    }

    public static Result<TError> Failure<TError>(TError error)
    {
        return new Result<TError>(false, error);
    }

    public static Result<TValue, Unit> Failure<TValue>()
    {
        return new Result<TValue, Unit>(false, default!, Unit.Value);
    }

    public static Result<Unit> Failure()
    {
        return new Result<Unit>(false, Unit.Value);
    }
}
