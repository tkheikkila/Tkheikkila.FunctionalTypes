using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for Maybe<T> implementation")]
public readonly partial struct Maybe<T>
{
	public static Maybe<T> Some(T value) => new(value);
    public static Maybe<T> None() => new();
    public static Maybe<T> None<TDiscard>(TDiscard _) => new();
}
