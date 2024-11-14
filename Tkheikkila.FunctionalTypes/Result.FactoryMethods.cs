using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required for Result<TValue, TError> implementation")]
public sealed partial class Result<TValue, TError>
{
	public static Result<TValue, TError> Ok(TValue value)
	{
		return new Result<TValue, TError>(true, value, default!);
	}

	public static Result<TValue, TError> Error(TError error)
	{
		return new Result<TValue, TError>(false, default!, error);
	}
}
