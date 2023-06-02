using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    private readonly TError _error;
    private readonly TValue _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    internal Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default!;
    }

    internal Result(TError error)
    {
        IsSuccess = false;
        _value = default!;
        _error = error;
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
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
            ? onSuccess(_value)
            : onFailure(_error);
    }

    #region Inspecting state

    public Result<TValue, TError> Peek(Action<TValue> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        if (IsSuccess)
        {
            onSuccess(_value);
        }

        return this;
    }

    public Result<TValue, TError> PeekError(Action<TError> onFailure)
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

    public bool IsSuccessWith(Func<TValue, bool> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess && onSuccess(_value);
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

    public TError GetErrorOrElse(Func<TValue, TError> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess(_value)
            : _error;
    }

    public TValue? GetValueOrDefault()
    {
        return IsSuccess
            ? _value
            : default;
    }

    public TValue GetValueOrDefault(TValue defaultValue)
    {
        return IsSuccess
            ? _value
            : defaultValue;
    }

    public TValue GetValueOrElse(Func<TError, TValue> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return IsSuccess
            ? _value
            : onFailure(_error);
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

    public bool TryGetValue(out TValue? value)
    {
        if (IsSuccess)
        {
            value = _value;
            return false;
        }

        value = default;
        return true;
    }

    public Maybe<TError> GetSomeValueOrNone()
    {
        return IsSuccess
            ? Maybe.None<TError>()
            : Maybe.Some(_error);
    }

    public Maybe<TError> GetSomeErrorOrNone()
    {
        return IsSuccess
            ? Maybe.None<TError>()
            : Maybe.Some(_error);
    }

    #endregion

    #region Map

    public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? Success(onSuccess(_value))
            : Failure<TResult>(_error);
    }

    public Result<TValue, TResult> MapError<TResult>(Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return IsSuccess
            ? Success<TResult>(_value)
            : Failure(onFailure(_error));
    }

    public TResult? MapOrDefault<TResult>(Func<TValue, TResult> onSuccess)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess(_value)
            : default;
    }

    public TResult MapOrDefault<TResult>(Func<TValue, TResult> onSuccess, TResult defaultValue)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        return IsSuccess
            ? onSuccess(_value)
            : defaultValue;
    }

    public TResult? MapErrorOrDefault<TResult>(Func<TError, TResult> onFailure)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return IsSuccess
            ? default
            : onFailure(_error);
    }

    public TResult MapErrorOrDefault<TResult>(Func<TError, TResult> onFailure, TResult defaultValue)
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return IsSuccess
            ? defaultValue
            : onFailure(_error);
    }

    #endregion

    #region FlatMap

    public Result<TError> FlatMap(Func<TValue, Result<TError>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? other(_value)
            : Result.Failure(_error);
    }

    public Result<TOther, TError> FlatMap<TOther>(Func<TValue, Result<TOther, TError>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? other(_value)
            : Failure<TOther>(_error);
    }

    public Result<TValue, TOther> FlatMapError<TOther>(Func<TError, Result<TOther>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? Result.Success<TValue, TOther>(_value)
            : other(_error).WithValue(_value);
    }

    public Result<TValue, TOther> FlatMapError<TOther>(Func<TError, Result<TValue, TOther>> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return IsSuccess
            ? Success<TOther>(_value)
            : other(_error);
    }

    #endregion

    #region Conversions

    public Result<TError, TValue> Invert()
    {
        return IsSuccess
            ? Result.Failure<TError, TValue>(_value)
            : Result.Success<TError, TValue>(_error);
    }

    public IEnumerable<TValue> AsEnumerable()
    {
        if (IsSuccess)
        {
            yield return _value;
        }
    }

    public IEnumerable<TError> AsErrorEnumerable()
    {
        if (IsFailure)
        {
            yield return _error;
        }
    }

    public Result<TError> WithoutValue()
    {
        return IsSuccess
            ? Result.Success<TError>()
            : Result.Failure(_error);
    }

    public override string ToString()
    {
        return IsSuccess
            ? $"Success({_value})"
            : $"Failure({_error})";
    }

    #endregion

    #region Equality

    public bool Equals(Result<TValue, TError>? other)
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
            (true, true)   => EqualityComparer<TValue>.Default.Equals(_value, other._value),
            (false, false) => EqualityComparer<TError>.Default.Equals(_error, other._error),
            _              => false
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Result<TValue, TError>);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsSuccess, _value, _error);
    }

    #endregion

    #region Operators

    public static bool operator ==(Result<TValue, TError>? left, Result<TValue, TError>? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Result<TValue, TError>? left, Result<TValue, TError>? right)
    {
        return !(left == right);
    }

    public static implicit operator Result<TValue, TError>(TValue value)
    {
        return Result.Success<TValue, TError>(value);
    }

    public static implicit operator Result<TValue, TError>(TError value)
    {
        return Result.Failure<TValue, TError>(value);
    }

    public static implicit operator Result<TError>(Result<TValue, TError> result)
    {
        return result.FlatMapError(Result.Failure);
    }

    public static bool operator true(Result<TValue, TError> result)
    {
        return result.IsSuccess;
    }

    public static bool operator false(Result<TValue, TError> result)
    {
        return result.IsFailure;
    }

    #endregion

    #region Helpers

    private static Result<TOther, TError> Success<TOther>(TOther other)
    {
        return Result.Success<TOther, TError>(other);
    }

    private static Result<TValue, TOther> Success<TOther>(TValue value)
    {
        return Result.Success<TValue, TOther>(value);
    }

    private static Result<TOther, TError> Failure<TOther>(TError error)
    {
        return Result.Failure<TOther, TError>(error);
    }

    private static Result<TValue, TOther> Failure<TOther>(TOther other)
    {
        return Result.Failure<TValue, TOther>(other);
    } 

    #endregion
}
