using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed partial class Result<TValue, TError> : IEquatable<Result<TValue, TError>>, IEquatable<TValue>
{
    private readonly TError _error;
    private readonly TValue _value;

    public bool HasValue { get; }
    public bool HasError => !HasValue;

    internal Result(bool hasValue, TValue value, TError error)
    {
        HasValue = hasValue;
        _value = HasValue ? value : default!;
        _error = HasValue ? default! : error;
    }

    public TResult Match<TResult>(Func<TValue, TResult> ok, Func<TError, TResult> error)
    {
        ok.ThrowIfNull(nameof(ok));
		error.ThrowIfNull(nameof(error));

		return HasValue
            ? ok(_value)
            : error(_error);
    }

    public void Match(Action<TValue> ok, Action<TError> error)
	{
		ok.ThrowIfNull(nameof(ok));
		error.ThrowIfNull(nameof(error));

		if (HasValue)
		{
			ok(_value);
		}
		else
		{
			error(_error);
		}
	}

	#region Extracting state

	public TError? GetErrorOrDefault()
        => HasValue
            ? default
            : _error;

    [return: NotNullIfNotNull(nameof(defaultValue))]
	public TError? GetErrorOrDefault(TError? defaultValue)
        => HasValue
            ? defaultValue
            : _error;

    public TError GetErrorOrElse(Func<TValue, TError> ok)
        => Match(ok, static error => error);

    public TValue? GetValueOrDefault()
        => GetValueOrDefault(default);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TValue? GetValueOrDefault(TValue? defaultValue)
        => HasValue
            ? _value
            : defaultValue;

    public TValue GetValueOrElse(Func<TError, TValue> error)
        => Match(static value => value, error);

    public bool TryGetError([MaybeNullWhen(false)] out TError error)
    {
        if (HasValue)
        {
            error = default;
            return false;
        }

        error = _error;
        return true;
    }

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

    public Maybe<TValue> GetValue()
        => Match(Maybe<TValue>.Some, Maybe<TValue>.None);

    public Maybe<TError> GetError()
        => Match(Maybe<TError>.None, Maybe<TError>.Some);

    #endregion

    #region Map

    public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> map)
    {
        map.ThrowIfNull(nameof(map));

		return FlatMap(value => Result<TResult, TError>.Ok(map(value)));
    }

    public Result<TValue, TResult> MapError<TResult>(Func<TError, TResult> map)
    {
		map.ThrowIfNull(nameof(map));

		return FlatMapError(e => Result<TValue, TResult>.Error(map(e)));
    }

    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> map)
        => MapOrDefault(map, default);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> map, TResult? defaultValue)
    {
        map.ThrowIfNull(nameof(map));

        return HasValue
            ? map(_value)
            : defaultValue;
    }

    public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> map)
        => MapErrorOrDefault(map, default);

    [return: NotNullIfNotNull(nameof(defaultValue))]
	public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> map, TResult? defaultValue)
    {
        map.ThrowIfNull(nameof(map));

        return HasValue
            ? defaultValue
            : map(_error);
    }

    #endregion

    #region FlatMap

    public Result<TOther, TError> FlatMap<TOther>(Func<TValue, Result<TOther, TError>> map)
        => Match(ok: map, error: Result<TOther, TError>.Error);

    public Result<TOther, TError> FlatMap<TIntermediate, TOther>(Func<TValue, Result<TIntermediate, TError>> map, Func<TValue, TIntermediate, TOther> flatMap)
        => FlatMap(value => map(value).Map(other => flatMap(value, other)));

    public Result<TValue, TOther> FlatMapError<TOther>(Func<TError, Result<TValue, TOther>> map)
        => Match(ok: Result<TValue, TOther>.Ok, error: map);

    public Result<TValue, TOther> FlatMapError<TIntermediate, TOther>(Func<TError, Result<TValue, TIntermediate>> map, Func<TError, TIntermediate, TOther> flatMap)
        => FlatMapError(error => map(error).MapError(other => flatMap(error, other)));

    #endregion

    #region Equality

    public bool Equals(Result<TValue, TError>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return (HasValue, other.HasValue) switch
        {
            (true, true)   => EqualityComparer<TValue>.Default.Equals(_value, other._value),
            (false, false) => EqualityComparer<TError>.Default.Equals(_error, other._error),
            _              => false
        };
    }

    public bool Equals(TValue? other)
        => HasValue && Equals(_value, other);

    public override bool Equals(object? obj)
        => obj switch
        {
            Result<TValue, TError> result => Equals(result),
            TValue value                  => Equals(value),
            _                             => false
        };

    public override int GetHashCode()
        => HashCode.Combine(HasValue, _value, _error);

    public static bool operator ==(Result<TValue, TError>? left, Result<TValue, TError>? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Result<TValue, TError>? left, Result<TValue, TError>? right)
        => !(left == right);

    public static bool operator ==(Result<TValue, TError>? left, TValue? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Result<TValue, TError>? left, TValue? right)
        => !(left == right);

    public static bool operator ==(TValue? left, Result<TValue, TError>? right)
        => right == left;

    public static bool operator !=(TValue? left, Result<TValue, TError>? right)
        => !(left == right);

    #endregion

    #region Conversions

    public override string ToString()
        => HasValue
            ? $"Ok({_value})"
            : $"Error({_error})";

    public static implicit operator Result<TValue, TError>(TValue value)
        => Ok(value);

    public static implicit operator Result<TValue, TError>(TError value)
        => Error(value);

	#endregion
}
