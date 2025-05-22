using System.Reflection;
using Cloudot.Shared.Attribute;
using Cloudot.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cloudot.Shared.EntityFramework.Interceptor;

public class SlugInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entity = entry.Entity;
            var type = entity.GetType();

            foreach (var targetProp in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = targetProp.GetCustomAttribute<SlugifyAttribute>();
                if (attr == null)
                    continue;

                var sourceProp = type.GetProperty(attr.From, BindingFlags.Public | BindingFlags.Instance);
                if (sourceProp == null)
                    continue;

                var sourceValue = sourceProp.GetValue(entity)?.ToString();
                var targetValue = targetProp.GetValue(entity)?.ToString();

                if (!string.IsNullOrWhiteSpace(sourceValue) && string.IsNullOrWhiteSpace(targetValue))
                {
                    var slug = sourceValue.ToSlug();
                    targetProp.SetValue(entity, slug);
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

