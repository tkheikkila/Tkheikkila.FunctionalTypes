using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly record struct Maybe<T>
{
    private readonly T _value;

    internal Maybe(bool hasValue, T value)
    {
        _value = value;
        HasValue = hasValue;
    }

    public bool HasValue { get; }

    public Maybe<TOther> And<TOther>(Maybe<TOther> other)
        => HasValue
            ? other
            : Maybe.None<TOther>();

    public IEnumerable<T> AsEnumerable()
    {
        if (HasValue)
            yield return _value;
    }

    public Maybe<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return HasValue && predicate(_value)
            ? Maybe.Some(_value)
            : Maybe.None<T>();
    }

    public Maybe<TOther> FlatMap<TOther>(Func<T, Maybe<TOther>> onSome)
    {
        if (onSome == null)
            throw new ArgumentNullException(nameof(onSome));

        return HasValue
            ? onSome(_value)
            : Maybe.None<TOther>();
    }

    public override int GetHashCode()
        => HashCode.Combine(_value, HasValue);

    public T? GetValueOrDefault()
        => HasValue
            ? _value
            : default;

    [return: NotNullIfNotNull("defaultValue")]
    public T? GetValueOrDefault(T? defaultValue)
        => HasValue
            ? _value
            : defaultValue;

    public T GetValueOrElse(Func<T> onNone)
    {
        if (onNone == null)
            throw new ArgumentNullException(nameof(onNone));

        return HasValue
            ? _value
            : onNone();
    }

    public T GetValueOrThrow() => HasValue
        ? _value
        : throw new InvalidOperationException("Optional value must contain a value.");

    public bool HasValueWith(Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return HasValue && predicate(_value);
    }

    public Maybe<T> Inspect(Action<T> onSome)
    {
        if (onSome == null)
            throw new ArgumentNullException(nameof(onSome));

        if (HasValue)
            onSome(_value);

        return this;
    }

    public Maybe<TResult> Map<TResult>(Func<T, TResult> onSome)
    {
        if (onSome == null)
            throw new ArgumentNullException(nameof(onSome));

        return HasValue
            ? Maybe.Some(onSome(_value))
            : Maybe.None<TResult>();
    }

    public Maybe<T> MapNone(Func<Maybe<T>> onNone)
    {
        if (onNone == null)
            throw new ArgumentNullException(nameof(onNone));

        return HasValue
            ? this
            : onNone();
    }

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        if (onNone == null)
            throw new ArgumentNullException(nameof(onNone));
        if (onSome == null)
            throw new ArgumentNullException(nameof(onSome));

        return HasValue
            ? onSome(_value)
            : onNone();
    }

    public Maybe<T> Or(Maybe<T> other)
        => HasValue
            ? this
            : other;

    public override string ToString()
        => HasValue
            ? $"Some({_value})"
            : "None";

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return HasValue;
    }

    public Maybe<T> Xor(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (true, false) => Maybe.Some(_value),
            (false, true) => Maybe.Some(other._value),
            _             => Maybe.None<T>()
        };

    public Maybe<TResult> Zip<TOther, TResult>(Maybe<TOther> other, Func<T, TOther, TResult> onBothSome)
    {
        if (onBothSome == null)
            throw new ArgumentNullException(nameof(onBothSome));

        return HasValue && other.HasValue
            ? Maybe.Some(onBothSome(_value, other._value))
            : Maybe.None<TResult>();
    }

    public bool Equals(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (true, true)   => Equals(_value, other._value),
            (false, false) => true,
            _              => false
        };

    public static explicit operator Maybe<Unit>(Maybe<T> other) => other.Map(_ => Unit.Value);

    public static explicit operator Maybe<T?>(Maybe<Unit> other) => other.Map(_ => default(T));

    public static explicit operator T(Maybe<T> other) => other.GetValueOrThrow();

    public static implicit operator Maybe<T>(T value) => Maybe.Some(value);
}