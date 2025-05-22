using Cloudot.Shared.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Shared.EntityFramework.Configurations;

public class BaseTimestampEntityConfiguration<TEntity> : BaseStatusEntityConfiguration<TEntity>
    where TEntity : class, ITimestampEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder); // BaseStatusEntityConfiguration'dan Id ayarı gelir

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.ModifiedDate)
            .IsRequired(false);

        builder.HasIndex(x => x.CreatedDate);
        builder.HasIndex(x => x.ModifiedDate);
    }
}