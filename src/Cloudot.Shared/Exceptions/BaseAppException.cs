namespace Cloudot.Shared.Exceptions;

public class BaseAppException : Exception
{
    public int StatusCode { get; }

    public BaseAppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}