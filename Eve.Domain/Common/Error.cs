namespace Eve.Domain.Common;
public class Error
{
    public static Error EmptyError = new Error(null, ErrorCodes.Empty);

    private readonly string _message;
    private readonly ErrorCodes _code;

    private Error(string? message, ErrorCodes errorCode)
    {
        _message = message;
        _code = errorCode;
    }

    public string? Message => _message;
    public ErrorCodes ErrorCode => _code;

    public static Error BadRequest(string? message= null)
    {
        message = message ?? "The sent data is not correct";
        return new(message, ErrorCodes.BadRequest);
    }

    public static Error Forbidden(string? message = null)
    {
        message = message ?? "This request cannot be completed due to a server-side restriction";
        return new(message, ErrorCodes.Forbidden);
    }

    public static Error NotFound (string? message= null)
    {
        message = message ?? "The sought entities were not found";
        return new(message, ErrorCodes.NotFound);
    }

    public static Error InternalServer (string? message= null)
    {
        message = message ?? "A server error occurred";
        return new(message, ErrorCodes.InternalServer);
    }

    public static Error NotModified (string? message= null)
    {
        message = message ?? "Not modified";
        return new(message, ErrorCodes.NotModified);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Error) return false;
        return Equals(obj as Error);
    }

    public bool Equals(Error other)
    {
        if (other == null) return false;
        return ErrorCode == other.ErrorCode && Message == other.Message;
    }
}

public enum ErrorCodes
{
    Empty = 0,
    NotModified = 304,
    BadRequest = 400,
    Forbidden = 403,
    NotFound = 404,
    InternalServer = 500,
}

