namespace Tkheikkila.FunctionalTypes;

public static class Validation
{
    public static LazyValidationResult<TValue, TError> Lazy<TValue, TError>(TValue value)
        => new(true, value, default!);

    public static GreedyValidationResult<TValue, TError> Greedy<TValue, TError>(TValue value)
        => new(true, value, Enumerable.Empty<TError>());
}
