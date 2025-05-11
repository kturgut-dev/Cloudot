using Cloudot.Shared.Entity;
using Cloudot.Shared.Enums;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Cloudot.Shared.EntityFramework;

public class EfRepository<TEntity>(BaseDbContext context) : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly BaseDbContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
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
}