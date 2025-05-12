using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository;

namespace Cloudot.Module.Management.Domain.User;

public interface IUserRepository : IEfRepository<User>
{
}