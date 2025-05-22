using System.Linq.Expressions;
using Cloudot.Shared.Entity;
using Cloudot.Shared.EntityFramework.Interceptor;
using Cloudot.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cloudot.Shared.EntityFramework;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options), IBaseDbContext
{
    public virtual string SchemaName => "public";

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IEntity
    {
        return Set<TEntity>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if !DESIGN_TIME
        // optionsBuilder.AddInterceptors(auditInterceptor);
        optionsBuilder.AddInterceptors(new SlugInterceptor());
        optionsBuilder.AddInterceptors(new SqlLoggingInterceptor());
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IStatusEntity).IsAssignableFrom(entityType.ClrType))
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