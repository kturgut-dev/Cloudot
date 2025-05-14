using System.IdentityModel.Tokens.Jwt;

namespace Cloudot.Infrastructure.Auth.Constants;

public static class AuthClaimTypes
{
    public const string UserId = JwtRegisteredClaimNames.Sub; //"sub";
    public const string TenantId = "tenant_id";
    public const string Email = JwtRegisteredClaimNames.Email; //"email";
}