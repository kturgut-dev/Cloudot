using System.Linq.Expressions;
using Cloudot.Shared.Entity;
using Cloudot.Shared.Enums;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Shared.EntityFramework;

public class EfRepository<TEntity, TContext>(TContext context) : IEfRepository<TEntity>
    where TEntity : class, IEntity
    where TContext : BaseDbContext
{
    private readonly TContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Ulid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(Ulid id, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        if (entity is IAuditEntity audit)
        {
            audit.Status = RecordStatus.Deleted;
            audit.ModifiedDate = DateTime.UtcNow;
            _context.Entry(entity).State = EntityState.Modified;
        }
        else
        {
            _dbSet.Remove(entity);
        }

        return true;
    }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> UpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TEntity>> updateExpression,
        CancellationToken cancellationToken = default)
    {
        TEntity? entity = await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        if (entity == null)
            return null;

        // UpdateExpression'i çalıştırmak için bir delegate'e çeviriyoruz
        Func<TEntity, TEntity> updateFunc = updateExpression.Compile();

        TEntity updatedEntity = updateFunc(entity);

        _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }
}