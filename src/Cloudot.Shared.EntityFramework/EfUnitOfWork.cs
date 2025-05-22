using Cloudot.Shared.Domain;
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


// public class EfUnitOfWork<TContext>(TContext context) : IUnitOfWork
//     where TContext : BaseDbContext
// {
//     public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//     {
//         return context.SaveChangesAsync(cancellationToken);
//     }
//
//     public DbSet<TEntity> Set<TEntity>() where TEntity : class
//     {
//         return context.Set<TEntity>();
//     }
// }


public class EfUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly IEventBus _eventBus;

    public EfUnitOfWork(TContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await _context.SaveChangesAsync(cancellationToken);

        // Domain Event'leri tetikle
        await _eventBus.DispatchAsync(_context);

        return result;
    }

    public DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>();
    }
}
