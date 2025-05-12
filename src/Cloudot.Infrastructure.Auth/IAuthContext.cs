using System.Security.Claims;

namespace Cloudot.Infrastructure.Auth;

public interface IAuthContext
{
    string? AccessToken { get; }
    ClaimsPrincipal? Principal { get; }
}
