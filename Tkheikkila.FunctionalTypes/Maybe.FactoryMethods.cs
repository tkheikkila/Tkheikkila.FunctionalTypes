namespace Tkheikkila.FunctionalTypes;

public static class Maybe
{
    public static Unit None()
    {
        return Unit.Value;
    }

    public static Maybe<T> None<T>()
    {
        return new Maybe<T>();
    }

    public static Maybe<T> None<T, TDiscard>(TDiscard _)
    {
        return None<T>();
    }

    public static Maybe<Unit> Some()
    {
        return new Maybe<Unit>(Unit.Value);
    }

    public static Maybe<T> Some<T>(T value)
    {
        return new Maybe<T>(value);
    }

    public static Task<Maybe<T>> NoneTask<T>()
    {
        return Task.FromResult(None<T>());
    }

    public static Task<Maybe<T>> SomeTask<T>(T value)
    {
        return Task.FromResult(Some(value));
    }

    public static ValueTask<Maybe<T>> NoneValueTask<T>()
    {
        return new ValueTask<Maybe<T>>(None<T>());
    }

    public static ValueTask<Maybe<T>> SomeValueTask<T>(T value)
    {
        return new ValueTask<Maybe<T>>(Some(value));
    }
}
