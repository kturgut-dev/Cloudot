namespace Cloudot.Shared.Exceptions;

public class UnauthorizedAppException : BaseAppException
{
    public UnauthorizedAppException(string message) : base(message, 401) { }
}