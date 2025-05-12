using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Cloudot.Infrastructure.Auth;

public class AuthContext(IHttpContextAccessor accessor) : IAuthContext
{
    public string? AccessToken =>
        accessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

    public ClaimsPrincipal? Principal => accessor.HttpContext?.User;
}
