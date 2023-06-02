using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly struct Maybe<T>
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

    public Maybe<T> Peek(Action<T> onSome)
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

    #endregion

    public Maybe<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && predicate(_value)
            ? Maybe.Some(_value)
            : Maybe.None<T>();
    }

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

    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (HasValue)
        {
            value = default;
            return false;
        }

        value = _value;
        return true;
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
            : Maybe.None<TResult>();
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
            : Maybe.None<TResult>();
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
            : Maybe.None<TOther>();
    }

    public Maybe<T> FlatMapNone(Func<Maybe<T>> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? this
            : onNone();
    }

    #endregion

    #region Conversions

    public Result<T, TError> AsSomeSuccessOr<TError>(TError errorOnNone)
    {
        return HasValue
            ? Result.Success<T, TError>(_value)
            : Result.Failure<T, TError>(errorOnNone);
    }

    public Result<T, TError> AsSomeSuccessOrElse<TError>(Func<TError> onNone)
    {
        if (onNone == null)
        {
            throw new ArgumentNullException(nameof(onNone));
        }

        return HasValue
            ? Result.Success<T, TError>(_value)
            : Result.Failure<T, TError>(onNone());
    }

    public Result<T> AsSomeFailure()
    {
        return HasValue
            ? Result.Failure(_value)
            : Result.Success<T>();
    }

    public Result<TValue, T> AsSomeFailureOr<TValue>(TValue valueOnNone)
    {
        return HasValue
            ? Result.Failure<TValue, T>(_value)
            : Result.Success<TValue, T>(valueOnNone);
    }

    public Result<TValue, T> AsSomeFailureOrElse<TValue>(Func<TValue> onNone)
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
            _              => false
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is Maybe<T> maybe && Equals(maybe);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HasValue, _value);
    }

    #endregion

    #region Operators

    public static explicit operator Maybe<Unit>(Maybe<T> other)
    {
        return other.Map(_ => Unit.Value);
    }

    public static explicit operator Maybe<T?>(Maybe<Unit> other)
    {
        return other.Map(_ => default(T));
    }

    public static implicit operator Maybe<T>(T value)
    {
        return Maybe.Some(value);
    }

    #endregion
}
