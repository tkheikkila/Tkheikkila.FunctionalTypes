namespace Tkheikkila.FunctionalTypes;

public sealed class GreedyValidationResult<TValue, TError>
{
	public bool IsValid { get; }
	private readonly TValue _value;
	private readonly IReadOnlyList<TError> _errors;

	internal GreedyValidationResult(bool isValid, TValue value, IEnumerable<TError> errors)
	{
		IsValid = isValid;
		_value = value;
		_errors = errors.ToArray();
	}

	public TResult Match<TResult>(Func<TValue, TResult> ok, Func<IReadOnlyList<TError>, TResult> errors)
	{
		ok.ThrowIfNull(nameof(ok));
		errors.ThrowIfNull(nameof(errors));

		return IsValid
			? ok(_value)
			: errors(_errors);
	}

	public void Match(Action<TValue> ok, Action<IReadOnlyList<TError>> errors)
	{
		ok.ThrowIfNull(nameof(ok));
		errors.ThrowIfNull(nameof(errors));

		if (IsValid)
		{
			ok(_value);
		}
		else
		{
			errors(_errors);
		}
	}

	public GreedyValidationResult<TValue, TError> AddError(TError error)
	{
		error.ThrowIfNull(nameof(error));

		return new GreedyValidationResult<TValue, TError>(false, _value, _errors.Append(error));
	}

	public GreedyValidationResult<TValue, TError> AddErrors(IEnumerable<TError> errors)
	{
		errors = errors as TError[] ?? errors as IReadOnlyCollection<TError> ?? errors?.ToArray()!;
		errors.ThrowIfNull(nameof(errors));
		errors.ThrowIfAnyNull(nameof(errors));

		return errors.ToArray() is { Length: > 0 } newErrors
			? new GreedyValidationResult<TValue, TError>(false, _value, _errors.Concat(newErrors))
			: this;
	}

	public GreedyValidationResult<TValue, TError> AddErrors(params TError[] errors)
	{
		errors.ThrowIfNull(nameof(errors));
		errors.ThrowIfAnyNull(nameof(errors));

		return errors.Length > 0
			? new GreedyValidationResult<TValue, TError>(false, _value, _errors.Concat(errors))
			: this;
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, IEnumerable<TError>> errors)
	{
		errors.ThrowIfNull(nameof(errors));

		return AddErrors(errors(_value));
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, GreedyValidationResult<TValue, TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return AddErrors(validation(_value)._errors);
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, Maybe<TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return validation(_value)
			.MapOrDefault(AddError, this);
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, Result<TValue, TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return validation(_value)
			.MapErrorOrDefault(AddError, this);
	}
}