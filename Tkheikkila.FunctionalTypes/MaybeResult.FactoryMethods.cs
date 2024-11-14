using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for MaybeResult<TValue, TError> implementation")]
public readonly partial struct MaybeResult<TValue, TError>
{
	public static MaybeResult<TValue, TError> Ok(TValue value)
	{
		return new MaybeResult<TValue, TError>(true, value, default!);
	}

	public static MaybeResult<TValue, TError> Ok(Maybe<TValue> value)
	{
		return value.Match(Ok, Neither);
	}

	public static MaybeResult<TValue, TError> OkOrNullAsNeither(TValue? value)
	{
		return OkOrNeither(value!, when: value is not null);
	}

	public static MaybeResult<TValue, TError> OkOrNeither(TValue value, bool when)
	{
		return when
			? Ok(value)
			: Neither();
	}

	public static MaybeResult<TValue, TError> OkOrNeither(TValue value, Func<TValue, bool> when)
	{
		return OkOrNeither(value, when(value));
	}

	public static MaybeResult<TValue, TError> Error(TError error)
	{
		return new MaybeResult<TValue, TError>(false, default!, error);
	}

	public static MaybeResult<TValue, TError> Error(Maybe<TError> error)
	{
		return error.Match(Error, Neither);
	}

	public static MaybeResult<TValue, TError> ErrorOrNullAsNeither(TError? error)
	{
		return ErrorOrNeither(error!, when: error is not null);
	}


	public static MaybeResult<TValue, TError> ErrorOrNeither(TError error, bool when)
	{
		return when
			? Error(error)
			: Neither();
	}

	public static MaybeResult<TValue, TError> ErrorOrNeither(TError error, Func<TError, bool> when)
	{
		return ErrorOrNeither(error, when(error));
	}


	public static MaybeResult<TValue, TError> Neither()
	{
		return new MaybeResult<TValue, TError>(null, default!, default!);
	}

	public static MaybeResult<TValue, TError> FromResult(Result<TValue, TError> result)
	{
		return result.Match(Ok, Error);
	}
}
