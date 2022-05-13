namespace Tkheikkila.FunctionalTypes;

public static class MaybeExtensions
{
    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> self)
        => self.GetValueOrElse(Maybe.None<T>);


    public static Result<Maybe<TValue>, TError> Transpose<TValue, TError>(this Maybe<Result<TValue, TError>> self)
    {
        return self.Match(
            SuccessWithSomeOrFailureWithError,
            SuccessWithNone
        );

        static Result<Maybe<TValue>, TError> SuccessWithNone()
            => Result.Success<Maybe<TValue>, TError>(Maybe.None<TValue>());

        static Result<Maybe<TValue>, TError> SuccessWithSomeOrFailureWithError(Result<TValue, TError> result)
            => result.Map(Maybe.Some);
    }

    public static Result<TValue, Maybe<TError>> TransposeError<TValue, TError>(this Maybe<Result<TValue, TError>> self)
    {
        return self.Match(
            FailureWithSomeOrSuccessWithValue,
            FailureWithNone
        );

        static Result<TValue, Maybe<TError>> FailureWithNone()
            => Result.Failure<TValue, Maybe<TError>>(Maybe.None<TError>());

        static Result<TValue, Maybe<TError>> FailureWithSomeOrSuccessWithValue(Result<TValue, TError> result)
            => result.MapError(Maybe.Some);
    }
}