using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class Result<TError> : IEquatable<Result<TError>>
{
	private readonly TError _error;

    public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;

	internal Result(bool isSuccess, TError error)
	{
		IsSuccess = isSuccess;
        _error = IsSuccess ? default! : error;
	}

	public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure)
	{
		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		return IsSuccess
			? onSuccess()
			: onFailure(_error);
	}

	#region Inspecting state

	public Result<TError> IfSuccess(Action onSuccess)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		if (IsSuccess)
		{
			onSuccess();
		}

		return this;
	}

	public Result<TError> IfFailure(Action<TError> onFailure)
	{
		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		if (IsFailure)
		{
			onFailure(_error);
		}

		return this;
	}

	public Result<TError> IfFailure<TSpecificError>(Action<TSpecificError> onSpecificFailure)
	{
		if (onSpecificFailure == null)
		{
			throw new ArgumentNullException(nameof(onSpecificFailure));
		}

		if (IsFailure && _error is TSpecificError specificError)
		{
			onSpecificFailure(specificError);
		}

		return this;
	}

	public bool IsFailureWith(Func<TError, bool> onError)
	{
		if (onError == null)
		{
			throw new ArgumentNullException(nameof(onError));
		}

		return IsFailure && onError(_error);
	}

    #endregion

	#region Extracting state

	public TError? GetErrorOrDefault()
	{
		return IsSuccess
			? default
			: _error;
	}

	public TError GetErrorOrDefault(TError defaultValue)
	{
		return IsSuccess
			? defaultValue
			: _error;
	}

	public TError GetErrorOrElse(Func<TError> onSuccess)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		return IsSuccess
			? onSuccess()
			: _error;
	}

	public TError GetErrorOrThrow()
	{
		return IsFailure
			? _error
			: throw new InvalidOperationException("Success does not have an error.");
	}

	public TError GetErrorOrThrow<TException>(Func<TException> onSuccess)
		where TException : Exception
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		if (IsFailure)
		{
			return _error;
		}

		throw onSuccess();
	}

    public bool TryGetError([MaybeNullWhen(false)] out TError error)
	{
		if (IsSuccess)
		{
			error = default;
			return false;
		}

		error = _error;
		return true;
	}
	 
	public Maybe<TError> GetError()
	{
		return IsSuccess
			? Maybe.None<TError>()
			: Maybe.Some(_error);
	}

	#endregion

	#region Setting state

	public Result<TOther, TError> ReplaceValue<TOther>(TOther value)
	{
		return IsSuccess
			? Success(value)
			: Failure<TOther>(_error);
	}

	public Result<TOther> ReplaceError<TOther>(TOther error)
	{
		return IsSuccess
			? Success<TOther>()
			: Failure(error);
	}

	#endregion

	#region Map

	public Result<TResult, TError> Map<TResult>(Func<TResult> onSuccess)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		return IsSuccess
			? Success(onSuccess())
			: Failure<TResult>(_error);
	}

	public Result<TMappedValue, TMappedError> Map<TMappedValue, TMappedError>(
		Func<TMappedValue> onSuccess,
		Func<TError, TMappedError> onFailure
	)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? Result.Success<TMappedValue, TMappedError>(onSuccess())
			: Result.Failure<TMappedValue, TMappedError>(onFailure(_error));
	}

	public Result<TResult> MapError<TResult>(Func<TError, TResult> onFailure)
	{
		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? Success<TResult>()
			: Failure(onFailure(_error));
	}

	public TResult? MapOrDefault<TResult>(Func<TResult> onSuccess)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		return IsSuccess
			? onSuccess()
			: default;
	}

	public TResult MapOrDefault<TResult>(Func<TResult> onSuccess, TResult defaultValue)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		return IsSuccess
			? onSuccess()
			: defaultValue;
	}

	public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> onFailure)
	{
		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? default
			: onFailure(_error);
	}

	public TResult MapErrorOrDefault<TResult>(Func<TError, TResult> onFailure, TResult defaultValue)
	{
		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? defaultValue
			: onFailure(_error);
	}

	#endregion

	#region MapAsync

	public ValueTask<Result<TResult, TError>> MapAsync<TResult>(Func<ValueTask<TResult>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return FlatMapAsync(async () => Result.Success<TResult, TError>(await map().ConfigureAwait(false)));
	}

	public async ValueTask<Result<TResult>> MapErrorAsync<TResult>(Func<TError, ValueTask<TResult>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? Result.Success<TResult>()
			: await map(_error).ConfigureAwait(false);
	}

	public async ValueTask<TResult?> MapOrDefaultAsync<TResult>(Func<ValueTask<TResult>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? await map().ConfigureAwait(false)
			: default;
	}

	public async ValueTask<TResult> MapOrDefaultAsync<TResult>(Func<ValueTask<TResult>> map, TResult defaultValue)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? await map().ConfigureAwait(false)
			: defaultValue;
	}

	public async ValueTask<TResult?> MapErrorOrDefaultAsync<TResult>(Func<TError, ValueTask<TResult>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? default
			: await map(_error).ConfigureAwait(false);
	}

	public async ValueTask<TResult> MapErrorOrDefaultAsync<TResult>(Func<TError, ValueTask<TResult>> map, TResult defaultValue)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? defaultValue
			: await map(_error).ConfigureAwait(false);
	}

	#endregion

	#region FlatMap

	public Result<TOther, TError> FlatMap<TOther>(Func<Result<TOther, TError>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? map()
			: Failure<TOther>(_error);
	}

	public Result<TMappedValue, TMappedError> FlatMap<TMappedValue, TMappedError>(Func<Result<TMappedValue, TMappedError>> onSuccess, Func<TError, Result<TMappedValue, TMappedError>> onFailure)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? onSuccess()
			: onFailure(_error);
	}

	public Result<TOther> FlatMapError<TOther>(Func<TError, Result<TOther>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? Success<TOther>()
			: map(_error);
	}

	#endregion

	#region FlatMapAsync

	public async ValueTask<Result<TOther, TError>> FlatMapAsync<TOther>(Func<ValueTask<Result<TOther, TError>>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? await map().ConfigureAwait(false)
			: Result.Failure<TOther, TError>(_error);
	}

	public async ValueTask<Result<TMappedValue, TMappedError>> FlatMapAsync<TMappedValue, TMappedError>(Func<ValueTask<Result<TMappedValue, TMappedError>>> onSuccess, Func<TError, ValueTask<Result<TMappedValue, TMappedError>>> onFailure)
	{
		if (onSuccess == null)
		{
			throw new ArgumentNullException(nameof(onSuccess));
		}

		if (onFailure == null)
		{
			throw new ArgumentNullException(nameof(onFailure));
		}

		return IsSuccess
			? await onSuccess().ConfigureAwait(false)
			: await onFailure(_error).ConfigureAwait(false);
	}

	public async ValueTask<Result<TOther>> FlatMapErrorAsync<TOther>(Func<TError, ValueTask<Result<TOther>>> map)
	{
		if (map == null)
		{
			throw new ArgumentNullException(nameof(map));
		}

		return IsSuccess
			? Result.Success<TOther>()
			: await map(_error).ConfigureAwait(false);
	}

	#endregion

	#region Conversions

	public IEnumerable<TError> AsErrorEnumerable()
	{
		if (IsFailure)
		{
			yield return _error;
		}
	}

	public override string ToString()
	{
		return IsSuccess
			? "Success"
			: $"Failure({_error})";
	}

	#endregion

	#region Equality

	public bool Equals(Result<TError>? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return (IsSuccess, other.IsSuccess) switch
		{
			(true, true) => true,
			(false, false) => EqualityComparer<TError>.Default.Equals(_error, other._error),
			_ => false
		};
	}

	public override bool Equals(object? obj)
	{
		return obj switch
		{
			Result<TError> result => Equals(result),
			_ => false
		};
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(IsSuccess, _error);
	}

	#endregion

	#region Operators

    public static implicit operator Maybe<TError>(Result<TError> result)
    {
        return result.GetError();
    }

    public static implicit operator Result<TError>(TError value)
	{
		return Result.Failure(value);
	}

	public static implicit operator Result<Unit, TError>(Result<TError> result)
	{
		return result.Map(() => Unit.Value);
	}

	public static implicit operator Result<Unit>(Result<TError> result)
	{
		return result.MapError(_ => Unit.Value);
	}

	public static implicit operator Result<Unit, Unit>(Result<TError> result)
	{
		return result.Map(
			() => Unit.Value,
			_ => Unit.Value
		);
	}

	public static implicit operator Result<TError>(Result<Unit, TError> result)
    {
        return result.Match(
			_ => Result.Success<TError>(),
			Result.Failure
        );
    }

	public static implicit operator Result<TError?>(Result<Unit> result)
	{
		return result.MapError(_ => default(TError));
	}

	public static implicit operator Result<TError?>(Result<Unit, Unit> result)
	{
		return result.Match(_ => Result.Success<TError?>(), _ => Result.Failure(default(TError)));
	}

	public static bool operator true(Result<TError> result)
	{
		return result.IsSuccess;
	}

	public static bool operator false(Result<TError> result)
	{
		return result.IsFailure;
	}

	#endregion

	#region Helpers

	private static Result<TOther, TError> Success<TOther>(TOther other)
	{
		return Result.Success<TOther, TError>(other);
	}

	private static Result<TOther> Success<TOther>()
	{
		return Result.Success<TOther>();
	}

	private static Result<TOther, TError> Failure<TOther>(TError error)
	{
		return Result.Failure<TOther, TError>(error);
	}

	private static Result<TOther> Failure<TOther>(TOther other)
	{
		return Result.Failure(other);
	}

	#endregion
}
