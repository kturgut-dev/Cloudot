using Microsoft.Extensions.DependencyInjection;

namespace Cloudot.Shared.EntityFramework.Seeding;

public static class SeederRunner
{
    public static async Task RunAllAsync(IServiceProvider serviceProvider)
    {
        IEnumerable<ISeeder> seeders = serviceProvider.GetServices<ISeeder>();

        foreach (ISeeder seeder in seeders)
        {
            await seeder.SeedAsync(serviceProvider);
        }
    }
}
