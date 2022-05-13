namespace Tkheikkila.FunctionalTypes;

public static class Maybe
{
    public static Maybe<T> None<T>() => new(false, default!);

    public static Maybe<T> Some<T>(T value) => new(true, value);
}