using Cloudot.Shared.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Shared.EntityFramework.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(
                v => v.ToByteArray(), // Ulid → byte[]
                v => new Ulid(v)) // byte[] → Ulid
            .HasColumnType("bytea");

        builder.Property<uint>(x => x.Version)
            .IsConcurrencyToken()
            .HasColumnName("xmin")
            .ValueGeneratedOnAddOrUpdate();
    }
}