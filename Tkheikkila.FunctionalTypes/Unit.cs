namespace Tkheikkila.FunctionalTypes;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by operator definition")]
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default!;

	public bool Equals(Unit other)
        => true;

    public override bool Equals(object? obj)
        => obj is Unit;

    public override int GetHashCode()
        => 0;

    public override string ToString()
        => "()";

    public static bool operator ==(Unit left, object? right)
        => right is Unit;

    public static bool operator !=(Unit left, object? right)
        => right is not Unit;

    public static bool operator ==(object? left, Unit right)
        => left is Unit;

    public static bool operator !=(object? left, Unit right)
        => left is not Unit;
}
