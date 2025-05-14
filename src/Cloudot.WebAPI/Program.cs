using Cloudot.Infrastructure.Auth;
using Cloudot.Infrastructure.Messaging;
using Cloudot.Infrastructure.Redis;
using Cloudot.Module.Management.Application;
using Cloudot.Module.Management.Infrastructure;
using Cloudot.Shared;
using Cloudot.Shared.EntityFramework;
using Cloudot.WebAPI.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCloudotShared();
builder.Services.AddEntityFrameworkShared();
builder.Services.AddRedisCacheManager(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddEmailSender();

builder.Services.AddManagementModule(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();