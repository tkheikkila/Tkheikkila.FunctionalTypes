namespace Tkheikkila.FunctionalTypes;

public readonly record struct Unit
{
    public static readonly Unit Value = default!;

    public override int GetHashCode() => 0;

    public override string ToString() => "_";

    public bool Equals(Unit other) => true;
}