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

	#region Extracting state

	public T? GetValueOrDefault()
	{
		return HasValue
			? _value
			: default;
	}

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T? GetValueOrDefault(T? defaultValue)
	{
		return HasValue
			? _value
			: defaultValue;
	}

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
	{
		map.ThrowIfNull(nameof(map));

		return FlatMap(value => Maybe<TResult>.Some(map(value)));
	}

	public TResult? MapOrDefault<TResult>(Func<T, TResult> map)
	{
		map.ThrowIfNull(nameof(map));

		return MapOrDefault(map, default);
	}

	[return: NotNullIfNotNull(nameof(valueOnNone))]
	public TResult? MapOrDefault<TResult>(Func<T, TResult> onSome, TResult? valueOnNone)
	{
		onSome.ThrowIfNull(nameof(onSome));

		return Match(
			onSome,
			() => valueOnNone
		);
	}

	#endregion

	#region FlatMap

	public Maybe<TOther> FlatMap<TOther>(Func<T, Maybe<TOther>> onSome)
	{
		onSome.ThrowIfNull(nameof(onSome));

		return Match(onSome, Maybe<TOther>.None);
	}

	public Maybe<TOther> FlatMapNone<TOther>(Func<Maybe<TOther>> onNone)
	{
		onNone.ThrowIfNull(nameof(onNone));

		return Match(Maybe<TOther>.None, onNone);
	}

	#endregion

	#region Equality

	public bool Equals(Maybe<T> other)
	{
		return (HasValue, other.HasValue) switch
		{
			(true, true) => Equals(_value, other._value),
			(false, false) => true,
			_ => false
		};
	}

	public bool Equals(T? other)
	{
		return HasValue && Equals(_value, other);
	}

	public override bool Equals(object? obj)
	{
		return obj switch
		{
			Maybe<T> maybe => Equals(maybe),
			T value => Equals(value),
			null => HasValue && _value is null,
			_ => false
		};
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(HasValue, _value);
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

	#region Conversions

	public override string ToString()
	{
		return HasValue
			? $"Some({_value})"
			: "None()";
	}

	public static implicit operator Maybe<T>(T value)
	{
		return Some(value);
	}

	public static implicit operator Maybe<T>(Unit _)
	{
		return None();
	}

	#endregion
}
