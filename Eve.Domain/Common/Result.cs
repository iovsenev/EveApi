using Eve.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Eve.Domain.Common;
public class Result
{
    
    public Result(
        bool isSuccess,
        Error error)
    {
        if (isSuccess && error != Error.EmptyError)
            throw new InvalidOperationException();
        if (!isSuccess && error == Error.EmptyError)
            throw new InvalidOperationException();

        Error = error;
        IsSuccess = isSuccess;
    }
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, Error.EmptyError);

    public static implicit operator Result(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    [JsonIgnore]
    public Error Error => base.Error;
    public TValue Value => IsSuccess && _value is not null
        ?_value
        : throw new NullReferenceResultValueException($"Value with type {typeof(TValue)} can not be null with success result");

    public static implicit operator Result<TValue>(Error error) => new(default, false, error);
    public static implicit operator Result<TValue>(TValue value) => new(value, true, Error.EmptyError);
}
