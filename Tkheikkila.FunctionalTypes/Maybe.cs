using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>
{
    private readonly T _value;

    public bool HasValue { get; }

    public Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public Maybe()
    {
        _value = default!;
        HasValue = false;
    }

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : onNone();
    }

    #region Inspecting state

    public bool HasValueWith(Func<T, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && predicate(_value);
    }

    public Maybe<T> IfSome(Action<T> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        if (HasValue)
        {
            onSome(_value);
        }

        return this;
    }

    public Maybe<T> IfNone(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        
        if (!HasValue)
        {
            action();
        }

        return this;
    }

    public Maybe<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && predicate(_value)
            ? Maybe.Some(_value)
            : Maybe.None();
    }

    public bool Contains(T value)
    {
        return HasValue && EqualityComparer<T>.Default.Equals(_value, value);
    }

    #endregion

    #region Extracting state

    public T? GetValueOrDefault()
    {
        return HasValue
            ? _value
            : default;
    }

    public T GetValueOrDefault(T defaultValue)
    {
        return HasValue
            ? _value
            : defaultValue;
    }

    public T GetValueOrElse(Func<T> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? _value
            : onNone();
    }

    public T GetValueOrThrow()
    {
        return HasValue
            ? _value
            : throw new InvalidOperationException("Maybe does not have a value.");
    }

    public T GetValueOrThrow<TException>(Func<TException> onNone) where TException : Exception
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        if (HasValue)
        {
            return _value;
        }

        throw onNone();
    }

    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (HasValue)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }

    #endregion

    #region Map

    public Maybe<TResult> Map<TResult>(Func<T, TResult> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? Maybe.Some(onSome(_value))
            : Maybe.None();
    }

    public TResult? MapOrDefault<TResult>(Func<T, TResult> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : default;
    }

    public TResult MapOrDefault<TResult>(Func<T, TResult> onSome, TResult valueOnNone)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : valueOnNone;
    }

    public Maybe<(T, TOther)> Zip<TOther>(Maybe<TOther> other)
    {
        return HasValue && other.HasValue
            ? Maybe.Some((_value, other._value))
            : Maybe.None();
    }

    public Maybe<TResult> Zip<TOther, TResult>(
        Maybe<TOther> other,
        Func<T, TOther, TResult> onBothSome
    )
    {
        if (onBothSome == null)
        {
            throw new ArgumentNullException(nameof(onBothSome));
        }

        return HasValue && other.HasValue
            ? Maybe.Some(onBothSome(_value, other._value))
            : Maybe.None();
    }

    #endregion

    #region MapAsync

    public async ValueTask<Maybe<TResult>> MapAsync<TResult>(Func<T, ValueTask<TResult>> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? Maybe.Some(await onSome(_value).ConfigureAwait(false))
            : Maybe.None();
    }

    public ValueTask<TResult?> MapOrDefaultAsync<TResult>(Func<T, ValueTask<TResult>> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return HasValue
            ? onSome(_value)
            : new ValueTask<TResult?>(default(TResult));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    public ValueTask<TResult> MapOrDefaultAsync<TResult>(Func<T, ValueTask<TResult>> onSome, TResult valueOnNone)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : new ValueTask<TResult>(valueOnNone);
    }

    public async ValueTask<Maybe<TResult>> ZipAsync<TOther, TResult>(Maybe<TOther> other, Func<T, TOther, ValueTask<TResult>> onBothSome)
    {
        if (onBothSome == null)
        {
            throw new ArgumentNullException(nameof(onBothSome));
        }

        return HasValue && other.HasValue
            ? Maybe.Some(await onBothSome(_value, other._value).ConfigureAwait(false))
            : Maybe.None();
    }

    #endregion

    #region FlatMap

    public Maybe<TOther> FlatMap<TOther>(Func<T, Maybe<TOther>> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : Maybe.None();
    }

    public Maybe<TOther> FlatMapNone<TOther>(Func<Maybe<TOther>> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? Maybe.None()
            : onNone();
    }

    #endregion

    #region FlatMapAsync

    public ValueTask<Maybe<TOther>> FlatMapAsync<TOther>(Func<T, ValueTask<Maybe<TOther>>> onSome)
    {
        if (onSome == null)
        {
            throw new ArgumentNullException(nameof(onSome));
        }

        return HasValue
            ? onSome(_value)
            : Maybe.NoneValueTask<TOther>();
    }

    public ValueTask<Maybe<TOther>> FlatMapNoneAsync<TOther>(Func<ValueTask<Maybe<TOther>>> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? Maybe.NoneValueTask<TOther>()
            : onNone();
    }

    #endregion

    #region Conversions

    public Result<T, Unit> AsSuccess()
    {
        return HasValue
            ? Result.Success<T, Unit>(_value)
            : Result.Failure<T, Unit>(Unit.Value);
    }

    public Result<T, TError> AsSuccessOr<TError>(TError errorOnNone)
    {
        return HasValue
            ? Result.Success<T, TError>(_value)
            : Result.Failure<T, TError>(errorOnNone);
    }

    public Result<T, TError> AsSuccessOrElse<TError>(Func<TError> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? Result.Success<T, TError>(_value)
            : Result.Failure<T, TError>(onNone());
    }

    public Result<Unit, T> AsFailure()
    {
        return HasValue
            ? Result.Failure<Unit, T>(_value)
            : Result.Success<Unit, T>(Unit.Value);
    }

    public Result<TValue, T> AsFailureOr<TValue>(TValue valueOnNone)
    {
        return HasValue
            ? Result.Failure<TValue, T>(_value)
            : Result.Success<TValue, T>(valueOnNone);
    }

    public Result<TValue, T> AsFailureOrElse<TValue>(Func<TValue> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? Result.Failure<TValue, T>(_value)
            : Result.Success<TValue, T>(onNone());
    }

    public IEnumerable<T> AsEnumerable()
    {
        if (HasValue)
        {
            yield return _value;
        }
    }

    public override string ToString()
    {
        return HasValue
            ? $"Some({_value})"
            : "None";
    }

    #endregion

    #region Equality

    public bool Equals(Maybe<T> other)
    {
        return (HasValue, other.HasValue) switch
        {
            (true, true)   => Equals(_value, other._value),
            (false, false) => true,
            _              => typeof(T) == typeof(Unit)
        };
    }

    public bool Equals(T? other)
    {
        return HasValue && Equals(_value, other) || typeof(T) == typeof(Unit);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Unit           => !HasValue || typeof(T) == typeof(Unit),
            Maybe<T> maybe => Equals(maybe),
            T value        => Equals(value),
            null           => HasValue && _value is null,
            _              => false
        };
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HasValue, _value);
    }

    #endregion

    #region Operators

    public static implicit operator Maybe<T>(T value)
    {
        return new Maybe<T>(value);
    }

    public static implicit operator Maybe<T>(Unit _)
    {
        return new Maybe<T>();
    }

    public static implicit operator Maybe<T?>(Maybe<Unit> maybe)
    {
        return maybe.Map(_ => default(T));
    }

    public static implicit operator Maybe<Unit>(Maybe<T> maybe)
    {
        return maybe.Map(_ => Unit.Value);
    }

    public static implicit operator Maybe<T>(Result<T, Unit> result)
    {
        return result.GetValue();
    }

    public static implicit operator Maybe<T>(Result<Unit, T> result)
    {
        return result.GetError();
    }

    public static implicit operator Result<T, Unit>(Maybe<T> maybe)
    {
        return maybe.AsSuccess();
    }

    public static implicit operator Result<Unit, T>(Maybe<T> maybe)
    {
        return maybe.AsFailure();
    }

    public static bool operator ==(Maybe<T> left, Maybe<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Maybe<T> left, Maybe<T> right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(Maybe<T> left, T right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Maybe<T> left, T right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(T left, Maybe<T> right)
    {
        return right.Equals(left);
    }

    public static bool operator !=(T left, Maybe<T> right)
    {
        return !right.Equals(left);
    }

    public static bool operator ==(Maybe<T> left, Unit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Maybe<T> left, Unit right)
    {
        return !(left == right);
    }

    public static bool operator ==(Unit left, Maybe<T> right)
    {
        return right == left;
    }

    public static bool operator !=(Unit left, Maybe<T> right)
    {
        return right != left;
    }

    #endregion
}
