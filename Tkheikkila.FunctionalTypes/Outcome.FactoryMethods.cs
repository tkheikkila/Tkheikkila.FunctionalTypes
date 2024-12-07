using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for Outcome<TValue, TWarning, TError> implementation")]
public sealed partial class Outcome<TValue, TWarning, TError>
{
    public static Outcome<TValue, TWarning, TError> Ok(TValue value)
    {
        return new Outcome<TValue, TWarning, TError>(Discriminator.Value, default!, default!, value);
    }

    public static Outcome<TValue, TWarning, TError> Warning(TWarning warning)
    {
        return new Outcome<TValue, TWarning, TError>(Discriminator.Warning, default!, warning, default!);
    }

    public static Outcome<TValue, TWarning, TError> Error(TError error)
    {
        return new Outcome<TValue, TWarning, TError>(Discriminator.Error, error, default!, default!);
    }
}
