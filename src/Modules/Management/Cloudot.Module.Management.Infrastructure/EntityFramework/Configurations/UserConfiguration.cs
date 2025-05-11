using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.Repository.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;

public class UserConfiguration : AuditBaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.IsRoot)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}