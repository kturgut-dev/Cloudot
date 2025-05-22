using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.EntityFramework.Seeding;
using Cloudot.Shared.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Module.Management.Infrastructure.EntityFramework.Seeding;

public class UserSeeder : ISeeder
{
    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IEfRepository<User> userRepository = scope.ServiceProvider.GetRequiredService<IEfRepository<User>>();

        string adminEmail = "admin@mail.com";

        bool exists = await userRepository.AnyAsync(x => x.Email == adminEmail);
        if (exists)
            return;

        User user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            IsActive = true,
            IsMailVerified = true,
            IsRoot = true
        };

        await userRepository.AddAsync(user);
        await unitOfWork.SaveChangesAsync();
    }
}