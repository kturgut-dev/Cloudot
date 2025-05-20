using System.Security.Claims;

namespace Cloudot.Infrastructure.Auth;

public interface ICurrentUser
{
    Ulid? Id { get; }
    string? Email { get; }
    string? Name { get; }
    List<Claim> Claims { get; }
    bool IsInRole(string role);
}
