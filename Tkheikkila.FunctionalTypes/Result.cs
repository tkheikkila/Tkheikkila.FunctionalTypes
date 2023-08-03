using System.Diagnostics.CodeAnalysis;

namespace Tkheikkila.FunctionalTypes;

public sealed class Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    private readonly TError _error;
    private readonly TValue _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default!;
    }

    public Result(TError error)
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

    public Result<TValue, TError> IfSuccess(Action<TValue> onSuccess)
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

    public Result<TValue, TError> IfFailure(Action<TError> onFailure)
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

    public TError GetErrorOrThrow()
    {
        return IsFailure
            ? _error
            : throw new InvalidOperationException("Success does not have an error.");
    }

    public TError GetErrorOrThrow<TException>(Func<TValue, TException> onSuccess)
        where TException : Exception
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        if (IsFailure)
        {
            return _error;
        }

        throw onSuccess(_value);
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

    public TValue GetValueOrThrow()
    {
        return IsSuccess
            ? _value
            : throw new InvalidOperationException("Failure does not have a value.");
    }

    public TValue GetValueOrThrow<TException>(Func<TError, TException> onFailure)
        where TException : Exception
    {
        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        if (IsSuccess)
        {
            return _value;
        }

        throw onFailure(_error);
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

    public Maybe<TValue> GetValue()
    {
        return IsSuccess
            ? Maybe.Some(_value)
            : Maybe.None<TValue>();
    }

    public Maybe<TError> GetError()
    {
        return IsSuccess
            ? Maybe.None<TError>()
            : Maybe.Some(_error);
    }

    #endregion

    #region Setting state

    public Result<TOther, TError> ReplaceValue<TOther>(TOther value)
    {
        return IsSuccess
            ? Success(value)
            : Failure<TOther>(_error);
    }

    public Result<TValue, TOther> ReplaceError<TOther>(TOther error)
    {
        return IsSuccess
            ? Success<TOther>(_value)
            : Failure(error);
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

    #region MapAsync

    public ValueTask<Result<TResult, TError>> MapAsync<TResult>(Func<TValue, ValueTask<TResult>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return FlatMapAsync(async value => Result.Success<TResult, TError>(await map(value).ConfigureAwait(false)));
    }

    public ValueTask<Result<TValue, TResult>> MapErrorAsync<TResult>(Func<TError, ValueTask<TResult>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return FlatMapErrorAsync(async error => Result.Failure<TValue, TResult>(await map(error).ConfigureAwait(false)));
    }

    public ValueTask<TResult?> MapOrDefaultAsync<TResult>(Func<TValue, ValueTask<TResult>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return IsSuccess
            ? map(_value)
            : new ValueTask<TResult?>(default(TResult?));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    public ValueTask<TResult> MapOrDefaultAsync<TResult>(Func<TValue, ValueTask<TResult>> map, TResult defaultValue)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? map(_value)
            : new ValueTask<TResult>(defaultValue);
    }

    public ValueTask<TResult?> MapErrorOrDefaultAsync<TResult>(Func<TError, ValueTask<TResult>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return IsSuccess
            ? new ValueTask<TResult?>(default(TResult?))
            : map(_error);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    public ValueTask<TResult> MapErrorOrDefaultAsync<TResult>(Func<TError, ValueTask<TResult>> map, TResult defaultValue)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? new ValueTask<TResult>(defaultValue)
            : map(_error);
    }

    #endregion

    #region FlatMap

    public Result<TOther, TError> FlatMap<TOther>(Func<TValue, Result<TOther, TError>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? map(_value)
            : Failure<TOther>(_error);
    }

    public Result<TValue, TOther> FlatMapError<TOther>(Func<TError, Result<TValue, TOther>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? Success<TOther>(_value)
            : map(_error);
    }

    #endregion

    #region FlatMapAsync

    public ValueTask<Result<TOther, TError>> FlatMapAsync<TOther>(Func<TValue, ValueTask<Result<TOther, TError>>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? map(_value)
            : Result.FailureValueTask<TOther, TError>(_error);
    }

    public ValueTask<Result<TValue, TOther>> FlatMapErrorAsync<TOther>(Func<TError, ValueTask<Result<TValue, TOther>>> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return IsSuccess
            ? Result.SuccessValueTask<TValue, TOther>(_value)
            : map(_error);
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

    public Result<Unit, TError> WithoutValue()
    {
        return IsSuccess
            ? Success(Unit.Value)
            : Failure<Unit>(_error);
    }

    public Result<TValue, Unit> WithoutError()
    {
        return IsSuccess
            ? Success<Unit>(_value)
            : Failure(Unit.Value);
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
        return obj switch
        {
            Result<TValue, TError> result => Equals(result),
            _                             => false
        };
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

    public static implicit operator Result<Unit, TError>(Result<TValue, TError> result)
    {
        return result.Map(_ => Unit.Value);
    }

    public static implicit operator Result<TValue, Unit>(Result<TValue, TError> result)
    {
        return result.MapError(_ => Unit.Value);
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
