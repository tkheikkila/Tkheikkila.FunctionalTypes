namespace Tkheikkila.FunctionalTypes;

internal static class Guard
{
	public static void ThrowIfNull<T>(this T value, string name)
	{
		if (value == null)
		{
			throw new ArgumentNullException(paramName: name, message: null);
		}
	}

	public static void ThrowIfAnyNull<T>(this IEnumerable<T> value, string name)
	{
		if (value.Any(x => x == null))
		{
			throw new ArgumentException(paramName: name, message: null);
		}
	}
}
