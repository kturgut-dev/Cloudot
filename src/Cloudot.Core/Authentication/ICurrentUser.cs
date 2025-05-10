using System.Security.Claims;

namespace Cloudot.Core.Authentication;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    string? Name { get; }
    List<Claim> Claims { get; }
    bool IsInRole(string role);
}
