using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for MaybeResult<TValue, TError> implementation")]
public readonly partial struct MaybeResult<TValue, TError>
{
    public static MaybeResult<TValue, TError> Ok(TValue value)
        => new(true, value, default!);

    public static MaybeResult<TValue, TError> Ok(Maybe<TValue> value)
        => value.Match(Ok, Neither);

    public static MaybeResult<TValue, TError> OkOrNullAsNeither(TValue? value)
        => OkOrNeither(value!, when: value is not null);

    public static MaybeResult<TValue, TError> OkOrNeither(TValue value, bool when)
        => when
            ? Ok(value)
            : Neither();

    public static MaybeResult<TValue, TError> OkOrNeither(TValue value, Func<TValue, bool> when)
        => OkOrNeither(value, when(value));

    public static MaybeResult<TValue, TError> Error(TError error)
        => new(false, default!, error);

    public static MaybeResult<TValue, TError> Error(Maybe<TError> error)
        => error.Match(Error, Neither);

    public static MaybeResult<TValue, TError> ErrorOrNullAsNeither(TError? error)
        => ErrorOrNeither(error!, when: error is not null);


    public static MaybeResult<TValue, TError> ErrorOrNeither(TError error, bool when)
        => when
            ? Error(error)
            : Neither();

    public static MaybeResult<TValue, TError> ErrorOrNeither(TError error, Func<TError, bool> when)
        => ErrorOrNeither(error, when(error));


    public static MaybeResult<TValue, TError> Neither()
        => new(null, default!, default!);

    public static MaybeResult<TValue, TError> FromResult(Result<TValue, TError> result)
        => result.Match(Ok, Error);
}
