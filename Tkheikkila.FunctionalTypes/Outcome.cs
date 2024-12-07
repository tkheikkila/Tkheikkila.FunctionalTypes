using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed partial class Outcome<TValue, TWarning, TError> : IEquatable<Outcome<TValue, TWarning, TError>>, IEquatable<TValue>
{
    private readonly TValue _value;
    private readonly TWarning _warning;
    private readonly TError _error;
    private readonly Discriminator _discriminator;

    private enum Discriminator
    {
        Value,
        Warning,
        Error
    }

    public bool HasValue => _discriminator == Discriminator.Value;
    public bool HasWarning => _discriminator == Discriminator.Warning;
    public bool HasError => _discriminator == Discriminator.Error;

    private Outcome(Discriminator discriminator, TError error, TWarning warning, TValue value)
    {
        _discriminator = discriminator;
        _error = discriminator == Discriminator.Error ? error : default!;
        _warning = discriminator == Discriminator.Warning ? warning : default!;
        _value = discriminator == Discriminator.Value ? value : default!;
    }

    public TResult Match<TResult>(Func<TValue, TResult> value, Func<TWarning, TResult> warning, Func<TError, TResult> error)
    {
        value.ThrowIfNull(nameof(value));
        warning.ThrowIfNull(nameof(warning));
        error.ThrowIfNull(nameof(error));

        return _discriminator switch
        {
            Discriminator.Value => value(_value),
            Discriminator.Warning => warning(_warning),
            Discriminator.Error => error(_error),
            _ => throw new InvalidOperationException("Invalid state") // Unreachable without shenanigans
        };
    }

    public void Match(Action<TValue> value, Action<TWarning> warning, Action<TError> error)
    {
        value.ThrowIfNull(nameof(value));
        warning.ThrowIfNull(nameof(warning));
        error.ThrowIfNull(nameof(error));

        switch (_discriminator)
        {
            case Discriminator.Value:
                value(_value);
                break;
            case Discriminator.Warning:
                warning(_warning);
                break;
            case Discriminator.Error:
                error(_error);
                break;
            default:
                throw new InvalidOperationException("Invalid state"); // Unreachable without shenanigans
        }
    }

    #region Extracting state

    public TValue? GetValueOrDefault()
    {
        return HasValue ? _value : default;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TValue? GetValueOrDefault(TValue? defaultValue)
    {
        return HasValue ? _value : defaultValue;
    }

    public TValue GetValueOrElse(Func<TWarning, TValue> warning, Func<TError, TValue> error)
    {
        return Match(static value => value, warning, error);
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
    {
        return HasValue ? Maybe<TValue>.Some(_value) : Maybe<TValue>.None();
    }

    public TWarning? GetWarningOrDefault()
    {
        return HasWarning ? _warning : default;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TWarning? GetWarningOrDefault(TWarning? defaultValue)
    {
        return HasWarning ? _warning : defaultValue;
    }

    public TWarning GetWarningOrElse(Func<TValue, TWarning> value, Func<TError, TWarning> error)
    {
        return Match(value, static warning => warning, error);
    }

    public bool TryGetWarning([MaybeNullWhen(false)] out TWarning warning)
    {
        if (HasWarning)
        {
            warning = _warning;
            return true;
        }
        warning = default;
        return false;
    }

    public Maybe<TWarning> GetWarning()
    {
        return HasWarning ? Maybe<TWarning>.Some(_warning) : Maybe<TWarning>.None();
    }

    public TError GetErrorOrDefault()
    {
        return HasError ? _error : default!;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TError GetErrorOrDefault(TError defaultValue)
    {
        return HasError ? _error : defaultValue;
    }

    public TError GetErrorOrElse(Func<TValue, TError> value, Func<TWarning, TError> warning)
    {
        return Match(value, warning, static error => error);
    }

    public bool TryGetError([MaybeNullWhen(false)] out TError error)
    {
        if (HasError)
        {
            error = _error;
            return true;
        }
        error = default!;
        return false;
    }

    public Maybe<TError> GetError()
    {
        return HasError ? Maybe<TError>.Some(_error) : Maybe<TError>.None();
    }

    #endregion Extracting state

    #region Map

    public Outcome<TResult, TWarning, TError> Map<TResult>(Func<TValue, TResult> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            value => Outcome<TResult, TWarning, TError>.Ok(map(value)),
            Outcome<TResult, TWarning, TError>.Warning,
            Outcome<TResult, TWarning, TError>.Error
        );
    }

    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> map)
    {
        map.ThrowIfNull(nameof(map));

        return MapOrDefault(map, default);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> map, TResult? defaultValue)
    {
        map.ThrowIfNull(nameof(map));

        return HasValue
            ? map(_value)
            : defaultValue;
    }

    public Outcome<TValue, TResult, TError> MapWarning<TResult>(Func<TWarning, TResult> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            Outcome<TValue, TResult, TError>.Ok,
            value => Outcome<TValue, TResult, TError>.Warning(map(value)),
            Outcome<TValue, TResult, TError>.Error
        );
    }

    public TResult? MapWarningOrDefault<TResult>(Func<TWarning, TResult> map)
    {
        map.ThrowIfNull(nameof(map));
        return MapWarningOrDefault(map, default);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TResult? MapWarningOrDefault<TResult>(Func<TWarning, TResult> map, TResult? defaultValue)
    {
        map.ThrowIfNull(nameof(map));

        return HasWarning
            ? map(_warning)
            : defaultValue;
    }

    public Outcome<TValue, TWarning, TResult> MapError<TResult>(Func<TError, TResult> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            Outcome<TValue, TWarning, TResult>.Ok,
            Outcome<TValue, TWarning, TResult>.Warning,
            error => Outcome<TValue, TWarning, TResult>.Error(map(error))
        );
    }

    public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> map)
    {
        map.ThrowIfNull(nameof(map));
        return MapErrorOrDefault(map, default);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> map, TResult? defaultValue)
    {
        map.ThrowIfNull(nameof(map));
        return HasError
            ? map(_error)
            : defaultValue;
    }

    #endregion Map

    #region FlatMap

    public Outcome<TResult, TWarning, TError> FlatMap<TResult>(Func<TValue, Outcome<TResult, TWarning, TError>> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            map,
            Outcome<TResult, TWarning, TError>.Warning,
            Outcome<TResult, TWarning, TError>.Error
        );
    }

    public Outcome<TValue, TResult, TError> FlatMapWarning<TResult>(Func<TWarning, Outcome<TValue, TResult, TError>> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            Outcome<TValue, TResult, TError>.Ok,
            map,
            Outcome<TValue, TResult, TError>.Error
        );
    }

    public Outcome<TValue, TWarning, TResult> FlatMapError<TResult>(Func<TError, Outcome<TValue, TWarning, TResult>> map)
    {
        map.ThrowIfNull(nameof(map));
        return Match(
            Outcome<TValue, TWarning, TResult>.Ok,
            Outcome<TValue, TWarning, TResult>.Warning,
            map
        );
    }

    #endregion FlatMap

    #region Equality

    public bool Equals(Outcome<TValue, TWarning, TError>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (_discriminator != other._discriminator)
        {
            return false;
        }

        return _discriminator switch
        {
            Discriminator.Value => EqualityComparer<TValue>.Default.Equals(_value, other._value),
            Discriminator.Warning => EqualityComparer<TWarning>.Default.Equals(_warning, other._warning),
            Discriminator.Error => EqualityComparer<TError>.Default.Equals(_error, other._error),
            _ => throw new InvalidOperationException("Invalid state") // Unreachable without shenanigans
        };
    }

    public bool Equals(TValue? other)
    {
        return HasValue && Equals(_value, other);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Outcome<TValue, TWarning, TError> result => Equals(result),
            TValue value => Equals(value),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            _discriminator,
            _value,
            _warning,
            _error
        );
    }

    public static bool operator ==(Outcome<TValue, TWarning, TError>? left, Outcome<TValue, TWarning, TError>? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Outcome<TValue, TWarning, TError>? left, Outcome<TValue, TWarning, TError>? right)
    {
        return !(left == right);
    }

    public static bool operator ==(Outcome<TValue, TWarning, TError>? left, TValue? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Outcome<TValue, TWarning, TError>? left, TValue? right)
    {
        return !(left == right);
    }

    public static bool operator ==(TValue? left, Outcome<TValue, TWarning, TError>? right)
    {
        return right == left;
    }

    public static bool operator !=(TValue? left, Outcome<TValue, TWarning, TError>? right)
    {
        return !(left == right);
    }

    #endregion Equality

    #region Conversions

    public Result<TValue, TError> ResolveWarning(Func<TWarning, Result<TValue, TError>> resolve)
    {
        resolve.ThrowIfNull(nameof(resolve));

        return Match(
            Result<TValue, TError>.Ok,
            resolve,
            Result<TValue, TError>.Error
        );
    }

    public override string ToString()
    {
        return Match(
            static value => $"Ok({value})",
            static warning => $"Warning({warning})",
            static error => $"Error({error})"
        );
    }

    public static implicit operator Outcome<TValue, TWarning, TError>(TValue value)
    {
        return Ok(value);
    }

    public static implicit operator Outcome<TValue, TWarning, TError>(TWarning warning)
    {
        return Warning(warning);
    }

    public static implicit operator Outcome<TValue, TWarning, TError>(TError error)
    {
        return Error(error);
    }

    #endregion
}
