namespace Tkheikkila.FunctionalTypes;

public static class ResultExtensions
{
    public static Result<TValue, TError> Flatten<TValue, TError>(this Result<Result<TValue, TError>, TError> self)
        => self.GetValueOrElse(Result.Failure<TValue, TError>);

    public static Result<TValue, TError> Flatten<TValue, TError>(this Result<TValue, Result<TValue, TError>> self)
        => self.GetErrorOrElse(Result.Success<TValue, TError>);

    public static Result<TValue, TError> Flatten<TValue, TError>(
        this Result<Result<TValue, TError>, Result<TValue, TError>> self
    )
    {
        return self.GetValueOrElse(e => e);
    }

    public static Maybe<Result<TValue, TError>> Transpose<TValue, TError>(this Result<Maybe<TValue>, TError> self)
    {
        return self.Match(
            SomeSuccessOrNone,
            SomeFailure
        );

        static Maybe<Result<TValue, TError>> SomeFailure(TError error)
            => Maybe.Some(Result.Failure<TValue, TError>(error));

        static Maybe<Result<TValue, TError>> SomeSuccessOrNone(Maybe<TValue> maybeValue)
            => maybeValue.Map(Result.Success<TValue, TError>);
    }

    public static Maybe<Result<TValue, TError>> TransposeError<TValue, TError>(this Result<TValue, Maybe<TError>> self)
    {
        return self.Match(
            SomeSuccess,
            SomeFailureOrNone
        );

        static Maybe<Result<TValue, TError>> SomeSuccess(TValue value)
            => Maybe.Some(Result.Success<TValue, TError>(value));

        static Maybe<Result<TValue, TError>> SomeFailureOrNone(Maybe<TError> maybeError)
            => maybeError.Map(Result.Failure<TValue, TError>); 
    }
}
