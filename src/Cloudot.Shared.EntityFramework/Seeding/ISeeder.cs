namespace Cloudot.Shared.EntityFramework.Seeding;

public interface ISeeder
{
    Task SeedAsync(IServiceProvider serviceProvider);
}
