using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly partial struct MaybeResult<TValue, TError> : IEquatable<MaybeResult<TValue, TError>>
{
	private readonly bool? _hasValue;
	private readonly TValue _value;
	private readonly TError _error;

	public bool HasValue => _hasValue is true;

	public bool HasError => _hasValue is false;

	public bool HasNeither => _hasValue is null;

	private MaybeResult(bool? hasValue, TValue value, TError error)
	{
		_hasValue = hasValue;
		_value = value;
		_error = error;
	}

	public TResult Match<TResult>(Func<TValue, TResult> ok, Func<TError, TResult> error, Func<TResult> none)
	{
		ok.ThrowIfNull(nameof(ok));
		error.ThrowIfNull(nameof(error));
		none.ThrowIfNull(nameof(none));

		return _hasValue switch
		{
			true => ok(_value),
			false => error(_error),
			null => none()
		};
	}

	public void Match(Action<TValue> ok, Action<TError> error, Action none)
	{
		ok.ThrowIfNull(nameof(ok));
		error.ThrowIfNull(nameof(error));
		none.ThrowIfNull(nameof(none));

		switch (_hasValue)
		{
			case true:
				ok(_value);
				break;

			case false:
				error(_error);
				break;

			case null:
				none();
				break;
		}
	}

	#region Extracting state

	public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
	{
		if (HasValue)
		{
			value = _value;
			return true;
		}

		value = default;
		return false;
	}

	public bool TryGetError([MaybeNullWhen(false)] out TError error)
	{
		if (HasError)
		{
			error = _error;
			return true;
		}

		error = default;
		return false;
	}

	public Maybe<TValue> GetValue()
	{
		return HasValue
			? _value
			: default!;
	}

	public Maybe<TError> GetError()
	{
		return HasError
			? _error
			: default!;
	}

	#endregion Extracting state

	#region Map

	public MaybeResult<TResult, TError> Map<TResult>(Func<TValue, TResult> map)
	{
		map.ThrowIfNull(nameof(map));

		return FlatMap(value => MaybeResult<TResult, TError>.Ok(map(value)));
	}

	public TResult? MapOrDefault<TResult>(Func<TValue, TResult> map)
	{
		map.ThrowIfNull(nameof(map));

		return MapOrDefault(map, default);
	}

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public TResult? MapOrDefault<TResult>(Func<TValue, TResult> onSome, TResult? defaultValue)
	{
		onSome.ThrowIfNull(nameof(onSome));

		return Match(onSome, _ => defaultValue, () => defaultValue);
	}

	#endregion Map

	#region FlatMap

	public MaybeResult<TResult, TError> FlatMap<TResult>(Func<TValue, MaybeResult<TResult, TError>> map)
	{
		map.ThrowIfNull(nameof(map));

		return Match(
			map,
			MaybeResult<TResult, TError>.Error,
			MaybeResult<TResult, TError>.Neither
		);
	}

	public MaybeResult<TValue, TResult> FlatMapError<TResult>(Func<TError, MaybeResult<TValue, TResult>> map)
	{
		map.ThrowIfNull(nameof(map));

		return Match(
			MaybeResult<TValue, TResult>.Ok,
			map,
			MaybeResult<TValue, TResult>.Neither
		);
	}

	public MaybeResult<TValue, TError> FlatMapNeither(Func<MaybeResult<TValue, TError>> map)
	{
		map.ThrowIfNull(nameof(map));

		return Match(Ok, Error, map);
	}

	#endregion FlatMap

	#region Equality

	public bool Equals(MaybeResult<TValue, TError> other)
	{
		return _hasValue == other._hasValue
			   && EqualityComparer<TValue>.Default.Equals(_value, other._value)
			   && EqualityComparer<TError>.Default.Equals(_error, other._error);
	}

	public override bool Equals(object? obj)
	{
		return obj is MaybeResult<TValue, TError> other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_hasValue, _value, _error);
	}

	public static bool operator ==(MaybeResult<TValue, TError> left, MaybeResult<TValue, TError> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MaybeResult<TValue, TError> left, MaybeResult<TValue, TError> right)
	{
		return !left.Equals(right);
	}

	#endregion Equality

	#region Conversions

	public override string ToString()
	{
		return Match(
			static ok => $"Ok({ok})",
			static error => $"Error({error})",
			static () => "Neither()"
		);
	}

	public static implicit operator MaybeResult<TValue, TError>(TValue value)
	{
		return Ok(value);
	}

	public static implicit operator MaybeResult<TValue, TError>(TError error)
	{
		return Error(error);
	}

	public static implicit operator MaybeResult<TValue, TError>(Maybe<TValue> maybe)
	{
		return Ok(maybe);
	}

	public static implicit operator MaybeResult<TValue, TError>(Maybe<TError> maybe)
	{
		return Error(maybe);
	}

	public static implicit operator MaybeResult<TValue, TError>(Result<TValue, TError> result)
	{
		return FromResult(result);
	}

	public static implicit operator MaybeResult<TValue, TError>(Maybe<Result<TValue, TError>> result)
	{
		return result.Match(FromResult, Neither);
	}

	public static implicit operator MaybeResult<TValue, TError>(Result<Maybe<TValue>, TError> result)
	{
		return result.Match(Ok, Error);
	}

	public static implicit operator MaybeResult<TValue, TError>(Result<TValue, Maybe<TError>> result)
	{
		return result.Match(Ok, Error);
	}

	public static explicit operator MaybeResult<TValue, TError>(Result<Maybe<TError>, Maybe<TValue>> result)
	{
		return result.Match(Error, Ok);
	}

	public static explicit operator MaybeResult<TValue, TError>(MaybeResult<TError, TValue> result)
	{
		return result.Match(Error, Ok, Neither);
	}

	public static explicit operator MaybeResult<TValue, TError>(Result<Maybe<TValue>, Maybe<TError>> result)
	{
		return result.Match(Ok, Error);
	}

	#endregion Conversions
}
