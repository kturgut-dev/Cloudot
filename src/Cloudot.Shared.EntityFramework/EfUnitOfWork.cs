using Cloudot.Shared.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Shared.EntityFramework;

// public class EfUnitOfWork(BaseDbContext context) : IUnitOfWork
// {
//     private readonly BaseDbContext _context = context;
//
//     public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//     {
//         return _context.SaveChangesAsync(cancellationToken);
//     }
// }


public class EfUnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : BaseDbContext
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return context.Set<TEntity>();
    }
}
