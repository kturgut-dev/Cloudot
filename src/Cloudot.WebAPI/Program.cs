using Cloudot.Infrastructure.Redis;
using Cloudot.Module.Management.Infrastructure;
using Cloudot.Shared;
using Cloudot.Shared.EntityFramework;
using Cloudot.Shared.Repository.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Servis kayıtları
builder.Services.AddControllers();
builder.Services.AddCloudotShared();
builder.Services.AddEntityFrameworkShared();
builder.Services.AddRedisCacheManager(builder.Configuration);
builder.Services.AddManagementModule(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();