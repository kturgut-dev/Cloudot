using Microsoft.Extensions.Localization;

namespace Cloudot.Shared.Exceptions;

public class ExceptionFactory(IStringLocalizer<ExceptionFactory> _localizer) : IExceptionFactory
{
    public BaseAppException Create(string key, int statusCode = 400)
    {
        string message = _localizer[key];
        return new BaseAppException(message, statusCode);
    }

    public NotFoundAppException NotFound(string key)
    {
        return new NotFoundAppException(_localizer[key]);
    }

    public UnauthorizedAppException Unauthorized(string key)
    {
        return new UnauthorizedAppException(_localizer[key]);
    }

    public ValidationAppException Validation(string key)
    {
        return new ValidationAppException(_localizer[key]);
    }
}
