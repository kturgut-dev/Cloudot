namespace Cloudot.Shared.Exceptions;

public interface IExceptionFactory
{
    BaseAppException Create(string key, int statusCode = 400);
    NotFoundAppException NotFound(string key);
    UnauthorizedAppException Unauthorized(string key);
    ValidationAppException Validation(string key);
}