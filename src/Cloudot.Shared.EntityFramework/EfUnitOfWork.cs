using Cloudot.Shared.Repository;
using Cloudot.Shared.Repository.EntityFramework;

namespace Cloudot.Shared.EntityFramework;

public class EfUnitOfWork(BaseDbContext context) : IUnitOfWork
{
    private readonly BaseDbContext _context = context;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}