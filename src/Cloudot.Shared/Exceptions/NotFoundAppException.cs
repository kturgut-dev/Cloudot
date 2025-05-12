namespace Cloudot.Shared.Exceptions;

public class NotFoundAppException : BaseAppException
{
    public NotFoundAppException(string message) : base(message, 404) { }
}