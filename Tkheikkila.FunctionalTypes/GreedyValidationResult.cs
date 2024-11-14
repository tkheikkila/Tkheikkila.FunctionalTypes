namespace Tkheikkila.FunctionalTypes;

public sealed class GreedyValidationResult<TValue, TError>
{
	public bool IsValid { get; }
	public TValue Value { get; }
	public IReadOnlyList<TError> Errors { get; }

	internal GreedyValidationResult(bool isValid, TValue value, IEnumerable<TError> errors)
	{
		IsValid = isValid;
		Value = value;
		Errors = errors.ToArray();
	}

	public TResult Match<TResult>(Func<TValue, TResult> ok, Func<IReadOnlyList<TError>, TResult> errors)
	{
		ok.ThrowIfNull(nameof(ok));
		errors.ThrowIfNull(nameof(errors));

		return IsValid
			? ok(Value)
			: errors(Errors);
	}

	public void Match(Action<TValue> ok, Action<IReadOnlyList<TError>> errors)
	{
		ok.ThrowIfNull(nameof(ok));
		errors.ThrowIfNull(nameof(errors));

		if (IsValid)
		{
			ok(Value);
		}
		else
		{
			errors(Errors);
		}
	}

	public GreedyValidationResult<TValue, TError> AddError(TError error)
	{
		error.ThrowIfNull(nameof(error));

		return new GreedyValidationResult<TValue, TError>(false, Value, Errors.Append(error));
	}

	public GreedyValidationResult<TValue, TError> AddErrors(IEnumerable<TError> errors)
	{
		errors = errors as TError[] ?? errors as IReadOnlyCollection<TError> ?? errors?.ToArray()!;
		errors.ThrowIfNull(nameof(errors));
		errors.ThrowIfAnyNull(nameof(errors));

		return errors.ToArray() is { Length: > 0 } newErrors
			? new GreedyValidationResult<TValue, TError>(false, Value, Errors.Concat(newErrors))
			: this;
	}

	public GreedyValidationResult<TValue, TError> AddErrors(params TError[] errors)
	{
		errors.ThrowIfNull(nameof(errors));
		errors.ThrowIfAnyNull(nameof(errors));

		return errors.Length > 0
			? new GreedyValidationResult<TValue, TError>(false, Value, Errors.Concat(errors))
			: this;
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, IEnumerable<TError>> errors)
	{
		errors.ThrowIfNull(nameof(errors));

		return AddErrors(errors(Value));
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, GreedyValidationResult<TValue, TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return AddErrors(validation(Value).Errors);
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, Maybe<TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return validation(Value)
			.MapOrDefault(AddError, this);
	}

	public GreedyValidationResult<TValue, TError> Validate(Func<TValue, Result<TValue, TError>> validation)
	{
		validation.ThrowIfNull(nameof(validation));

		return validation(Value)
			.MapErrorOrDefault(AddError, this);
	}
}