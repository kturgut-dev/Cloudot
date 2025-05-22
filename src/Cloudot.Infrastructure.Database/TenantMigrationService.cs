using System.Reflection;
using Cloudot.Shared.Domain;
using Cloudot.Shared.EntityFramework.Tenant;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Cloudot.Infrastructure.Database;

public class TenantMigrationService : ITenantMigrationService
{
    public async Task MigrateAllAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        var dbContextTypes = GetDbContextTypes();
        
        if (!dbContextTypes.Any())
        {
            // Log warning: No DbContext types found for migration
            return;
        }

        foreach (var contextType in dbContextTypes)
        {
            try
            {
                await MigrateContextAsync(contextType, connectionString, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error for specific context type
                throw new InvalidOperationException($"Failed to migrate context type: {contextType.FullName}", ex);
            }
        }
    }

    private static List<Type> GetDbContextTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && 
                              assembly.FullName?.StartsWith("Cloudot.Module") == true)
            .SelectMany(assembly => 
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    // Skip assemblies that can't be loaded
                    return Array.Empty<Type>();
                }
            })
            .Where(type => !type.IsAbstract &&
                          typeof(DbContext).IsAssignableFrom(type) &&
                          typeof(IDbContextMigrationMarker).IsAssignableFrom(type))
            .ToList();
    }

    private static async Task MigrateContextAsync(Type contextType, string connectionString, CancellationToken cancellationToken)
    {
        // Create generic DbContextOptionsBuilder<TContext>
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(contextType);
        var optionsBuilder = Activator.CreateInstance(optionsBuilderType);
        
        if (optionsBuilder == null)
            throw new InvalidOperationException($"Failed to create DbContextOptionsBuilder for type: {contextType.FullName}");

        // Cast to base DbContextOptionsBuilder to use UseNpgsql
        var baseOptionsBuilder = (DbContextOptionsBuilder)optionsBuilder;
        baseOptionsBuilder.UseNpgsql(connectionString);

        // Get DbContextOptions from builder using specific property resolution
        var optionsProperty = optionsBuilderType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => p.Name == "Options" && p.PropertyType.IsGenericType && 
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbContextOptions<>));
        
        if (optionsProperty == null)
        {
            // Fallback: try getting Options property without generic constraint
            optionsProperty = optionsBuilderType.GetProperty("Options", 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }
        
        if (optionsProperty == null)
            throw new InvalidOperationException($"Options property not found on {optionsBuilderType.FullName}");

        var options = optionsProperty.GetValue(optionsBuilder);
        if (options == null)
            throw new InvalidOperationException("Failed to get DbContextOptions from builder");

        // Verify the constructor exists
        var constructor = contextType.GetConstructor(new[] { options.GetType() });
        if (constructor == null)
        {
            // Try with base DbContextOptions type
            constructor = contextType.GetConstructor(new[] { typeof(DbContextOptions) });
            if (constructor == null)
                throw new InvalidOperationException($"No suitable constructor found for {contextType.FullName}. Expected constructor with DbContextOptions parameter.");
        }

        // Create DbContext instance
        var contextInstance = Activator.CreateInstance(contextType, options);
        if (contextInstance is not DbContext dbContext)
            throw new InvalidOperationException($"Failed to create DbContext instance for type: {contextType.FullName}");

        try
        {
            // Log migration attempt
            Console.WriteLine($"Migrating schema: {GetSchemaName(dbContext)} for context: {contextType.Name}");
            
            await dbContext.Database.MigrateAsync(cancellationToken);
            
            Console.WriteLine($"Migration completed for: {contextType.Name}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Migration failed for context {contextType.Name}: {ex.Message}", ex);
        }
        finally
        {
            await dbContext.DisposeAsync();
        }
    }

    private static string GetSchemaName(DbContext dbContext)
    {
        // Try to get schema name if it's a BaseDbContext
        var schemaProperty = dbContext.GetType().GetProperty("SchemaName", BindingFlags.Public | BindingFlags.Instance);
        return schemaProperty?.GetValue(dbContext)?.ToString() ?? "public";
    }
}