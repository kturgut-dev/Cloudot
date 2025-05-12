using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

string? connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<ManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
ManagementDbContext db = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
await db.Database.MigrateAsync();