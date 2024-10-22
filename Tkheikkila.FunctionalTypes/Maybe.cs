using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly partial struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>
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
        onNone.ThrowIfNull(nameof(onNone));
		onSome.ThrowIfNull(nameof(onSome));

        return HasValue
            ? onSome(_value)
            : onNone();
    }

    public void Match(Action<T> onSome, Action onNone)
	{
		onNone.ThrowIfNull(nameof(onNone));
		onSome.ThrowIfNull(nameof(onSome));

		if (HasValue)
		{
			onSome(_value);
		}
		else
		{
			onNone();
		}
	}

	#region Inspecting state

	public Maybe<T> Filter(Func<T, bool> predicate)
    {
		predicate.ThrowIfNull(nameof(predicate));

		return HasValue && predicate(_value)
            ? Some(_value)
            : None();
    }

    public bool Contains(T value)
        => HasValue && EqualityComparer<T>.Default.Equals(_value, value);

    #endregion

    #region Extracting state

    public T? GetValueOrDefault()
        => HasValue
            ? _value
            : default;

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public T? GetValueOrDefault(T? defaultValue)
        => HasValue
            ? _value
            : defaultValue;

    public T GetValueOrElse(Func<T> onNone)
    {
		onNone.ThrowIfNull(nameof(onNone));

		return HasValue
            ? _value
            : onNone();
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

    public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
        => FlatMap(value => Maybe<TResult>.Some(map(value)));

    public TResult? MapOrDefault<TResult>(Func<T, TResult> map)
        => MapOrDefault(map, default);

    [return: NotNullIfNotNull(nameof(valueOnNone))]
    public TResult? MapOrDefault<TResult>(Func<T, TResult> onSome, TResult? valueOnNone)
        => Match(
            onSome,
            () => valueOnNone
        );

    #endregion

    #region FlatMap

    public Maybe<TOther> FlatMap<TOther>(Func<T, Maybe<TOther>> onSome)
        => Match(onSome, Maybe<TOther>.None);

    public Maybe<TOther> FlatMapNone<TOther>(Func<Maybe<TOther>> onNone)
        => Match(Maybe<TOther>.None, onNone);

    #endregion

    #region Equality

    public bool Equals(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (true, true)   => Equals(_value, other._value),
            (false, false) => true,
            _              => false
        };

    public bool Equals(T? other)
        => HasValue && Equals(_value, other);

    public override bool Equals(object? obj)
        => obj switch
        {
            Maybe<T> maybe => Equals(maybe),
            T value        => Equals(value),
            null           => HasValue && _value is null,
            _              => false
        };

    public override int GetHashCode()
        => HashCode.Combine(HasValue, _value);

    public static bool operator ==(Maybe<T> left, Maybe<T> right)
        => left.Equals(right);

    public static bool operator !=(Maybe<T> left, Maybe<T> right)
        => !left.Equals(right);

    public static bool operator ==(Maybe<T> left, T right)
        => left.Equals(right);

    public static bool operator !=(Maybe<T> left, T right)
        => !left.Equals(right);

    public static bool operator ==(T left, Maybe<T> right)
        => right.Equals(left);

    public static bool operator !=(T left, Maybe<T> right)
        => !right.Equals(left);

    public static bool operator ==(Maybe<T> left, Unit right)
        => left.Equals(right);

    public static bool operator !=(Maybe<T> left, Unit right)
        => !(left == right);

    public static bool operator ==(Unit left, Maybe<T> right)
        => right == left;

    public static bool operator !=(Unit left, Maybe<T> right)
        => right != left;

    #endregion

    #region Conversions

    public override string ToString()
        => HasValue
            ? $"Some({_value})"
            : "None";

    public static implicit operator Maybe<T>(T value)
        => Some(value);

    public static implicit operator Maybe<T>(Unit _)
        => None();

    #endregion
}
