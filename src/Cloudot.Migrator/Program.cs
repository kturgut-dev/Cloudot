using Cloudot.Module.Management.Domain.LocalizationRecord;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Cloudot.Module.Management.Infrastructure.EntityFramework.Seeding;
using Cloudot.Shared.Domain;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.EntityFramework.Seeding;
using Cloudot.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

string? connectionString = builder.Configuration.GetConnectionString("Default");


// ManagementDbContext kayıtları
builder.Services.AddDbContext<ManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork<ManagementDbContext>>();

builder.Services.AddScoped<IEfRepository<User>>(provider =>
{
    ManagementDbContext context = provider.GetRequiredService<ManagementDbContext>();
    return new EfRepository<User, ManagementDbContext>(context);
});

builder.Services.AddScoped<IEfRepository<LocalizationRecord>>(provider =>
{
    ManagementDbContext context = provider.GetRequiredService<ManagementDbContext>();
    return new EfRepository<LocalizationRecord, ManagementDbContext>(context);
});

builder.Services.AddScoped<IEventBus, InMemoryEventBus>();

// Seeder kayıtları
builder.Services.AddScoped<ISeeder, UserSeeder>();
builder.Services.AddScoped<ISeeder, LocalizationRecordSeeder>();

IHost app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();

// Migrate
ManagementDbContext db = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
await db.Database.MigrateAsync();

// Seed
await SeederRunner.RunAllAsync(scope.ServiceProvider);


// var app = builder.Build();
//
// using IServiceScope scope = app.Services.CreateScope();
// ManagementDbContext db = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
// await db.Database.MigrateAsync();