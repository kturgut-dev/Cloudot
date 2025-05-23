using Microsoft.EntityFrameworkCore;

namespace Cloudot.Shared.Repository;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}