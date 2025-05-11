using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository.EntityFramework;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;

public class UserRepository(BaseDbContext context) : EfRepository<User>(context), IUserRepository;