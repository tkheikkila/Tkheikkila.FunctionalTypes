namespace Tkheikkila.FunctionalTypes;

internal static class Guard
{
	public static void ThrowIfNull<T>(this T value, string name)
	{
		if (value == null)
		{
			throw new ArgumentNullException(name);
		}
	}
}
