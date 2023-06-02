namespace Tkheikkila.FunctionalTypes;

public readonly record struct Unit
{
    public static readonly Unit Value = default!;

    public bool Equals(Unit other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "_";
    }
}
