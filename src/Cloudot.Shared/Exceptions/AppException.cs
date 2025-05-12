namespace Cloudot.Shared.Exceptions;

public class AppException : BaseAppException
{
    public AppException(string message, int statusCode = 500) : base(message, statusCode) { }
}