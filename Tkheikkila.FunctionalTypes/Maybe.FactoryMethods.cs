using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for Maybe<T> implementation")]
public readonly partial struct Maybe<T>
{
	public static Maybe<T> Some(T value)
	{
		return new Maybe<T>(value);
	}

	public static Maybe<T> SomeNullAsNone(T? value)
	{
		return SomeOrNone(value!, when: value is not null);
	}

	public static Maybe<T> SomeOrNone(T value, bool when)
	{
		return when
			? Some(value)
			: None();
	}

	public static Maybe<T> SomeOrNone(T value, Func<T, bool> when)
	{
		return SomeOrNone(value, when(value));
	}

	public static Maybe<T> None()
	{
		return new Maybe<T>();
	}

	public static Maybe<T> None<TDiscard>(TDiscard _)
	{
		return new Maybe<T>();
	}
}
