namespace Cloudot.Shared.Exceptions;

public class ValidationAppException : BaseAppException
{
    public ValidationAppException(string message) : base(message, 400) { }
}
