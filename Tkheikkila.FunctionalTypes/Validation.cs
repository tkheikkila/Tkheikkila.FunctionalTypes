namespace Tkheikkila.FunctionalTypes;

public static class Validation
{
	public static LazyValidationResult<TValue, TError> Lazy<TValue, TError>(TValue value)
	{
		return new LazyValidationResult<TValue, TError>(true, value, default!);
	}

	public static GreedyValidationResult<TValue, TError> Greedy<TValue, TError>(TValue value)
	{
		return new GreedyValidationResult<TValue, TError>(true, value, []);
	}
}
