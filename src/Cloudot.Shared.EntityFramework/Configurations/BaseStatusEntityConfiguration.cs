using Cloudot.Shared.Entity;
using Cloudot.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Shared.EntityFramework.Configurations;

public class BaseStatusEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : class, IStatusEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder); // BaseEntityConfiguration'dan Id ayarı gelir

        builder.Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(RecordStatus.Active)
            .HasConversion<int>();
    }
}