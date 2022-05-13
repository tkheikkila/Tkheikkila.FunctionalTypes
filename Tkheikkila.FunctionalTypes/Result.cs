using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public readonly record struct Result<TValue, TError>
{
    private readonly TError _error;
    private readonly TValue _value;

    internal Result(bool isSuccess, TValue value, TError error)
    {
        _value = value;
        _error = error;
        IsSuccess = isSuccess;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsSuccess { get; }

    public Result<TOther, TError> And<TOther>(Result<TOther, TError> other)
        => IsSuccess
            ? other
            : Result.Failure<TOther, TError>(_error);

    public Result<TOther, TError> AndThen<TOther>(Func<TValue, Result<TOther, TError>> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return IsSuccess
            ? other(_value)
            : Result.Failure<TOther, TError>(_error);
    }

    public TError? GetErrorOrDefault() => IsSuccess
        ? default
        : _error;

    [return: NotNullIfNotNull("defaultOnSuccess")]
    public TError? GetErrorOrDefault(TError? defaultOnSuccess) => IsSuccess
        ? defaultOnSuccess
        : _error;

    public TError GetErrorOrElse(Func<TValue, TError> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        return IsSuccess
            ? onSuccess(_value)
            : _error;
    }

    public TError GetErrorOrThrow()
        => IsSuccess
            ? _error
            : throw new InvalidOperationException("Cannot unwrap error when Success.");

    public override int GetHashCode()
        => HashCode.Combine(IsSuccess, _value, _error);

    public TValue? GetValueOrDefault() => IsSuccess
        ? _value
        : default;

    [return: NotNullIfNotNull("defaultOnFailure")]
    public TValue? GetValueOrDefault(TValue? defaultOnFailure) => IsSuccess
        ? _value
        : defaultOnFailure;

    public TValue GetValueOrElse(Func<TError, TValue> onFailure)
    {
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));

        return IsSuccess
            ? _value
            : onFailure(_error);
    }

    public TValue GetValueOrThrow()
        => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot unwrap value when Failure.");

    public Result<TValue, TError> Inspect(Action<TValue> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        if (IsSuccess)
            onSuccess(_value);

        return this;
    }

    public Result<TValue, TError> InspectError(Action<TError> onFailure)
    {
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));

        if (!IsSuccess)
            onFailure(_error);

        return this;
    }

    public bool IsFailureWith(Func<TError, bool> onError)
    {
        if (onError == null)
            throw new ArgumentNullException(nameof(onError));

        return !IsSuccess && onError(_error);
    }

    public bool IsSuccessWith(Func<TValue, bool> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        return IsSuccess && onSuccess(_value);
    }

    public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        return IsSuccess
            ? Result.Success<TResult, TError>(onSuccess(_value))
            : Result.Failure<TResult, TError>(_error);
    }

    public Result<TValue, TResult> MapError<TResult>(Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));

        return IsSuccess
            ? Result.Success<TValue, TResult>(_value)
            : Result.Failure<TValue, TResult>(onFailure(_error));
    }

    [return: NotNullIfNotNull("valueOnFailure")]
    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> onSuccess, TResult? valueOnFailure = default)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        return IsSuccess
            ? onSuccess(_value)
            : valueOnFailure;
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));

        return IsSuccess
            ? onSuccess(_value)
            : onFailure(_error);
    }

    public Result<TValue, TOther> Or<TOther>(Result<TValue, TOther> other)
        => IsSuccess
            ? Result.Success<TValue, TOther>(_value)
            : other;

    public Result<TValue, TOther> OrElse<TOther>(Func<TError, Result<TValue, TOther>> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return IsSuccess
            ? Result.Success<TValue, TOther>(_value)
            : other(_error);
    }

    public override string ToString()
        => IsSuccess
            ? $"Success({_value})"
            : $"Failure({_error})";

    public bool TryGetError(out TError? error)
    {
        error = _error;
        return !IsSuccess;
    }

    public bool TryGetValue(out TValue? value)
    {
        value = _value;
        return IsSuccess;
    }

    public IEnumerable<TValue> AsEnumerable()
    {
        if (IsSuccess)
            yield return _value;
    }

    public IEnumerable<TError> AsErrorEnumerable()
    {
        if (!IsSuccess)
            yield return _error;
    }

    public bool Equals(Result<TValue, TError> other)
        => (IsSuccess, other.IsSuccess) switch
        {
            (true, true)   => Equals(_value, other._value),
            (false, false) => Equals(_error, other._error),
            _              => false
        };
}