using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository;

namespace Cloudot.Module.Management.Domain.User;

public interface IUserEfRepository : IEfRepository<User>
{
    Task<User?> GetByEmailCompiledAsync(string email, CancellationToken cancellationToken = default);
}