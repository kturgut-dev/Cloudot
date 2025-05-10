using System.Security.Claims;

namespace Cloudot.Core.Authentication;

public interface IAuthContext
{
    string? AccessToken { get; }
    ClaimsPrincipal? Principal { get; }
}
