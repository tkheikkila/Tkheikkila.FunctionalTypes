using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class LazyValidationResult<TValue, TError>
{
    public bool IsValid { get; }
    private readonly TValue _value;
    private readonly TError _error;

    internal LazyValidationResult(bool isValid, TValue value, TError error)
    {
        IsValid = isValid;
        _value = value;
        _error = error;
    }

    public TResult Match<TResult>(Func<TValue, TResult> ok, Func<TError, TResult> errors)
    {
        ok.ThrowIfNull(nameof(ok));
        errors.ThrowIfNull(nameof(errors));

        return IsValid
            ? ok(_value)
            : errors(_error);
    }

    public void Match(Action<TValue> ok, Action<TError> error)
    {
        ok.ThrowIfNull(nameof(ok));
        error.ThrowIfNull(nameof(error));

        if (IsValid)
        {
            ok(_value);
        }
        else
        {
            error(_error);
        }
    }

    public bool TryGetError([MaybeNullWhen(false)] out TError errors)
    {
        if (IsValid)
        {
            errors = default;
            return false;
        }

        errors = _error;
        return true;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, IEnumerable<TError>> validation)
    {
        validation.ThrowIfNull(nameof(validation));

        if (IsValid && validation(_value).FirstOrDefault() is { } error)
        {
            return Error(error);
        }

        return this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, LazyValidationResult<TValue, TError>> validation)
    {
        validation.ThrowIfNull(nameof(validation));

        return IsValid
            ? validation(_value)
            : this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, Maybe<TError>> validation)
    {
        validation.ThrowIfNull(nameof(validation));

        return IsValid
            ? validation(_value).MapOrDefault(Error, this)
            : this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, Result<TValue, TError>> validation)
    {
        validation.ThrowIfNull(nameof(validation));

        return IsValid
            ? validation(_value).MapErrorOrDefault(Error, this)
            : this;
    }

    private static LazyValidationResult<TValue, TError> Error(TError error)
    {
        return new LazyValidationResult<TValue, TError>(false, default!, error);
    }
}
