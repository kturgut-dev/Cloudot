using System.Linq.Expressions;
using Cloudot.Shared.Entity;
using Cloudot.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cloudot.Shared.EntityFramework;

public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (EntityEntry<IAuditEntity> entry in ChangeTracker.Entries<IAuditEntity>())
        {
            DateTime now = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    entry.Entity.Status = RecordStatus.Active;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDate = now;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.Status = RecordStatus.Deleted;
                    entry.Entity.ModifiedDate = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            // if (typeof(IHasDomainEvents).IsAssignableFrom(entityType.ClrType))
            // {
            //     modelBuilder.Entity(entityType.ClrType)
            //         .Ignore(nameof(IHasDomainEvents.DomainEvents));
            // }
            
            if (typeof(IAuditEntity).IsAssignableFrom(entityType.ClrType))
            {
                ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "e");
                MemberExpression statusProperty = Expression.Property(parameter, nameof(IAuditEntity.Status));
                ConstantExpression deletedConstant = Expression.Constant(RecordStatus.Deleted);
                BinaryExpression notDeleted = Expression.NotEqual(statusProperty, deletedConstant);

                LambdaExpression filter = Expression.Lambda(notDeleted, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}