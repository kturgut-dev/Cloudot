using Cloudot.Module.Management.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Cloudot.Module.Management.Infrastructure.Localization;

public class DbStringLocalizerFactory(IServiceProvider provider) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
        => provider.GetRequiredService<DbStringLocalizer>();

    public IStringLocalizer Create(string baseName, string location)
        => provider.GetRequiredService<DbStringLocalizer>();
}
