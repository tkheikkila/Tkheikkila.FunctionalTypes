namespace Tkheikkila.FunctionalTypes;

public static class Result
{
    public static Result<TValue, TError> Failure<TValue, TError>(TError error) => new(false, default!, error);

    public static Result<TValue, TError> Success<TValue, TError>(TValue value) => new(true, value, default!);

    public static Func<Result<TValue, TError>> RaiseSuccess<TValue, TError>(Func<TValue> f)
        => () => Success<TValue, TError>(f());

    public static Func<T, Result<TValue, TError>> RaiseSuccess<T, TValue, TError>(Func<T, TValue> f)
        => x => Success<TValue, TError>(f(x));

    public static Func<Result<TValue, TError>> RaiseFailure<TValue, TError>(Func<TError> f)
        => () => Failure<TValue, TError>(f());

    public static Func<T, Result<TValue, TError>> RaiseFailure<T, TValue, TError>(Func<T, TError> f)
        => x => Failure<TValue, TError>(f(x));
}