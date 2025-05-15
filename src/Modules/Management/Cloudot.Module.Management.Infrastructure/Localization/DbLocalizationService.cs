using Cloudot.Core.Utilities.Caching;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Cloudot.Module.Management.Infrastructure.Localization;

public class DbStringLocalizer(IServiceProvider serviceProvider) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            string culture = Thread.CurrentThread.CurrentUICulture.Name;
            string cacheKey = $"localization:{culture}:{name}";

            using IServiceScope scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
            var cacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager>();

            string? value = cacheManager
                .GetOrAddAsync(cacheKey, async () =>
                {
                    return await context.LocalizationRecords
                        .AsNoTracking()
                        .Where(x => x.Culture == culture && x.Key == name)
                        .Select(x => x.Value)
                        .FirstOrDefaultAsync();
                }, TimeSpan.FromHours(1)).GetAwaiter().GetResult();

            return value is null
                ? new LocalizedString(name, name, true)
                : new LocalizedString(name, value, false);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(this[name].Value, arguments), false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => Enumerable.Empty<LocalizedString>();

    public IStringLocalizer WithCulture(System.Globalization.CultureInfo culture)
        => this;
}