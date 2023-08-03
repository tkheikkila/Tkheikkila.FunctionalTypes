using System.Net.Http.Headers;

namespace Tkheikkila.FunctionalTypes;

public static class Result
{
    public static Result<TValue, TError> Success<TValue, TError>(TValue value)
    {
        return new Result<TValue, TError>(value);
    }

    public static Result<TValue, Unit> Success<TValue>(TValue value)
    {
        return new Result<TValue, Unit>(value);
    }

    public static Result<Unit, TError> Success<TError>()
    {
        return new Result<Unit, TError>(Unit.Value);
    }

    public static Task<Result<TValue, TError>> SuccessTask<TValue, TError>(TValue value)
    {
        return Task.FromResult(Success<TValue, TError>(value));
    }

    public static Task<Result<Unit, TError>> SuccessTask<TError>()
    {
        return Task.FromResult(Success<TError>());
    }

    public static Task<Result<TValue, Unit>> SuccessTask<TValue>(TValue value)
    {
        return Task.FromResult(Success(value));
    }

    public static ValueTask<Result<TValue, TError>> SuccessValueTask<TValue, TError>(TValue value)
    {
        return new ValueTask<Result<TValue, TError>>(Success<TValue, TError>(value));
    }

    public static ValueTask<Result<Unit, TError>> SuccessValueTask<TError>()
    {
        return new ValueTask<Result<Unit, TError>>(Success<TError>());
    }

    public static ValueTask<Result<TValue, Unit>> SuccessValueTask<TValue>(TValue value)
    {
        return new ValueTask<Result<TValue, Unit>>(Success(value));
    }

    public static Result<TValue, TError> Failure<TValue, TError>(TError error)
    {
        return new Result<TValue, TError>(error);
    }

    public static Result<Unit, TError> Failure<TError>(TError error)
    {
        return new Result<Unit, TError>(error);
    }

    public static Result<TValue, Unit> Failure<TValue>()
    {
        return new Result<TValue, Unit>(Unit.Value);
    }

    public static Task<Result<TValue, TError>> FailureTask<TValue, TError>(TError error)
    {
        return Task.FromResult(Failure<TValue, TError>(error));
    }

    public static Task<Result<Unit, TError>> FailureTask<TError>(TError error)
    {
        return Task.FromResult(Failure(error));
    }

    public static Task<Result<TValue, Unit>> FailureTask<TValue>()
    {
        return Task.FromResult(Failure<TValue>());
    }


    public static ValueTask<Result<TValue, TError>> FailureValueTask<TValue, TError>(TError error)
    {
        return new ValueTask<Result<TValue, TError>>(Failure<TValue, TError>(error));
    }

    public static ValueTask<Result<Unit, TError>> FailureValueTask<TError>(TError error)
    {
        return new ValueTask<Result<Unit, TError>>(Failure(error));
    }

    public static ValueTask<Result<TValue, Unit>> FailureValueTask<TValue>()
    {
        return new ValueTask<Result<TValue, Unit>>(Failure<TValue>());
    }
}
