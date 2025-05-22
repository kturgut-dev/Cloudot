using Cloudot.Shared.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Shared.EntityFramework.Configurations;

public abstract class BaseAuditEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : class, IAuditEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder); // BaseEntityConfiguration'dan Id ayarı gelir

        builder.Property(x => x.CreatedBy)
            .IsRequired(false);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.ModifiedBy)
            .IsRequired(false);

        builder.Property(x => x.ModifiedDate)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<byte>();

        builder.HasIndex(x => x.Status);
    }
}