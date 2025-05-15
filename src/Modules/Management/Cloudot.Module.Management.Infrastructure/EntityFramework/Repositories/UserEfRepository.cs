using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository.EntityFramework;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;

public class UserEfRepository(ManagementDbContext context) : EfRepository<User>(context), IUserEfRepository;