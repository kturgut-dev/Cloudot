using System.Security.Claims;
using Cloudot.Infrastructure.Auth.Constants;
using Microsoft.AspNetCore.Http;

namespace Cloudot.Infrastructure.Auth;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private readonly ClaimsPrincipal? _user = accessor.HttpContext?.User;

    public Ulid?  Id =>
        GetClaim(AuthClaimTypes.UserId) is { } idStr && Ulid.TryParse(idStr, out var guid)
            ? guid : null;

    public string? Name => _user?.Identity?.Name;
    public string? Email => GetClaim(AuthClaimTypes.Email);
    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;
    public List<Claim> Claims => _user?.Claims.ToList() ?? [];

    public bool IsInRole(string role) => _user?.IsInRole(role) ?? false;

    public string? GetClaim(string claimType) =>
        _user?.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
}