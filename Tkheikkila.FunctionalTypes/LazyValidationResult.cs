using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class LazyValidationResult<TValue, TError>
{
    public bool IsValid { get; }
    public TValue Value { get; }
    private readonly TError _error;

    internal LazyValidationResult(bool isValid, TValue value, TError error)
    {
        IsValid = isValid;
        Value = value;
        _error = error;
    }

    public TResult Match<TResult>(Func<TValue, TResult> ok, Func<TError, TResult> errors)
    {
        ok.ThrowIfNull(nameof(ok));
        errors.ThrowIfNull(nameof(errors));

        return IsValid
            ? ok(Value)
            : errors(_error);
    }

    public void Match(Action<TValue> ok, Action<TError> errors)
    {
        ok.ThrowIfNull(nameof(ok));
        errors.ThrowIfNull(nameof(errors));

        if (IsValid)
        {
            ok(Value);
        }
        else
        {
            errors(_error);
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
        if (IsValid && validation(Value).FirstOrDefault() is { } error)
        {
            return Error(error);
        }

        return this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, LazyValidationResult<TValue, TError>> validation)
    {
        return IsValid
            ? validation(Value)
            : this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, Maybe<TError>> validation)
    {
        return IsValid
            ? validation(Value).MapOrDefault(Error, this)
            : this;
    }

    public LazyValidationResult<TValue, TError> Validate(Func<TValue, Result<TValue, TError>> validation)
    {
        return IsValid
            ? validation(Value).MapErrorOrDefault(Error, this)
            : this;
    }

    private static LazyValidationResult<TValue, TError> Error(TError error) => new(false, default!, error);
}
