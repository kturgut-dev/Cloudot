using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework.Configurations;
using Cloudot.Shared.Repository.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
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
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsMailVerified)
            .IsRequired()
            .HasDefaultValue(false);
    }
}