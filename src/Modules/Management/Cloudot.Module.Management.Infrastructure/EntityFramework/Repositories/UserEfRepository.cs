using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Repositories;

public class UserEfRepository(ManagementDbContext context) : EfRepository<User,ManagementDbContext>(context), IUserEfRepository
{
    private static readonly Func<ManagementDbContext, string, Task<User?>> _getByEmailCompiled =
        EF.CompileAsyncQuery((ManagementDbContext context, string email) =>
            context.Set<User>().SingleOrDefault(u => u.Email.Equals(email)));

    public Task<User?> GetByEmailCompiledAsync(string email, CancellationToken cancellationToken = default)
    {
        return _getByEmailCompiled(context, email);
    }
}