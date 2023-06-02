using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class Result<TError> : IEquatable<Result<TError>>
{
    private readonly TError _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    internal Result()
    {
        IsSuccess = true;
        _error = default!;
    }

    internal Result(TError error)
    {
        IsSuccess = false;
        _error = error;
    }

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess()
            : onFailure(_error);
    }

    #region Inspecting state

    public Result<TError> Peek(Action onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        if (IsSuccess)
        {
            onSuccess();
        }

        return this;
    }

    public Result<TError> PeekError(Action<TError> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        if (IsFailure)
        {
            onFailure(_error);
        }

        return this;
    }

    public bool IsFailureWith(Func<TError, bool> onError)
    {
        if (onError == null)
        {
            throw new ArgumentNullException(nameof(onError));
        }

        return IsFailure && onError(_error);
    }

    public bool IsSuccessWith(Func<bool> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess && onSuccess();
    }

    #endregion

    #region Extracting state

    public TError? GetErrorOrDefault()
    {
        return IsSuccess
            ? default
            : _error;
    }

    public TError GetErrorOrDefault(TError defaultValue)
    {
        return IsSuccess
            ? defaultValue
            : _error;
    }

    public TError GetErrorOrElse(Func<TError> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess()
            : _error;
    }

    public bool TryGetError([MaybeNullWhen(false)] out TError error)
    {
        if (IsSuccess)
        {
            error = default;
            return false;
        }

        error = _error;
        return true;
    }

    public Maybe<TError> GetSomeErrorOrNone()
    {
        return IsSuccess
            ? Maybe.None<TError>()
            : Maybe.Some(_error);
    }

    #endregion

    #region Map

    public Result<TResult, TError> Map<TResult>(Func<TResult> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? Result.Success<TResult, TError>(onSuccess())
            : Result.Failure<TResult, TError>(_error);
    }

    public Result<TResult> MapError<TResult>(Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return IsSuccess
            ? Result.Success<TResult>()
            : Result.Failure(onFailure(_error));
    }

    public TResult? MapOrDefault<TResult>(Func<TResult> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess()
            : default;
    }

    public TResult MapOrDefault<TResult>(Func<TResult> onSuccess, TResult defaultValue)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess()
            : defaultValue;
    }

    #endregion

    #region FlatMap

    public Result<TError> FlatMap(Func<Result<TError>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? other()
            : Result.Failure(_error);
    }

    public Result<TOther, TError> FlatMap<TOther>(Func<Result<TOther, TError>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? other()
            : Result.Failure<TOther, TError>(_error);
    }

    public Result<TOther> FlatMapError<TOther>(Func<TError, Result<TOther>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? Result.Success<TOther>()
            : other(_error);
    }

    public Result<TError> FlatMapError(Func<TError, Result<TError>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? Result.Success<TError>()
            : other(_error);
    }

    #endregion

    #region Conversions

    public Result<TValue, TError> WithValue<TValue>(TValue value)
    {
        return IsSuccess
            ? Result.Success<TValue, TError>(value)
            : Result.Failure<TValue, TError>(_error);
    }

    public IEnumerable<TError> AsErrorEnumerable()
    {
        if (IsFailure)
        {
            yield return _error;
        }
    }

    public override string ToString()
    {
        return IsSuccess
            ? "Success"
            : $"Failure({_error})";
    }

    #endregion

    #region Equality

    public bool Equals(Result<TError>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return (IsSuccess, other.IsSuccess) switch
        {
            (true, true)   => true,
            (false, false) => EqualityComparer<TError>.Default.Equals(_error, other._error),
            _              => false
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<TError> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsSuccess, _error);
    }

    #endregion

    #region Operators

    public static implicit operator Result<TError>(Unit _)
    {
        return Result.Success<TError>();
    }

    public static implicit operator Result<TError>(TError value)
    {
        return Result.Failure(value);
    }

    public static bool operator true(Result<TError> result)
    {
        return result.IsSuccess;
    }

    public static bool operator false(Result<TError> result)
    {
        return result.IsFailure;
    }

    #endregion
}
