namespace Tkheikkila.FunctionalTypes;

public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default!;

	public bool Equals(Unit other)
    {
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Unit;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "_";
    }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by operator definition")]
	public static bool operator ==(Unit left, object? right)
    {
        return right is Unit;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by operator definition")]
    public static bool operator !=(Unit left, object? right)
    {
        return right is not Unit;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by operator definition")]
    public static bool operator ==(object? left, Unit right)
    {
        return left is Unit;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by operator definition")]
    public static bool operator !=(object? left, Unit right)
    {
        return left is not Unit;
    }
}
