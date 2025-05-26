using Cloudot.Infrastructure.Auth;
using Cloudot.Shared.Entity;
using Cloudot.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cloudot.Shared.EntityFramework.Interceptor;

public class AuditSaveChangesInterceptor(ICurrentUser currentUserService) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext context = eventData.Context!;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        DateTime now = DateTime.UtcNow;
        Guid? currentUser = currentUserService.Id;

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            if (entry.Entity is ITimestampEntity timestamp)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        timestamp.CreatedDate = now;
                        if (entry.Entity is IStatusEntity statusEntity)
                            statusEntity.RecordStatus = RecordStatus.Active;
                        break;

                    case EntityState.Modified:
                        timestamp.ModifiedDate = now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        if (entry.Entity is IStatusEntity deletedStatusEntity)
                            deletedStatusEntity.RecordStatus = RecordStatus.Deleted;
                        timestamp.ModifiedDate = now;
                        break;
                }
            }

            if (entry.Entity is IAuditEntity audit)
            {
                if (entry.State == EntityState.Added)
                    audit.CreatedBy = currentUser;

                audit.ModifiedBy = currentUser;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}