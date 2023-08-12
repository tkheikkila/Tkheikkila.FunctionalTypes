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
}
