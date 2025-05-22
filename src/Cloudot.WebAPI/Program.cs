using System.Reflection;
using Cloudot.Infrastructure.Auth;
using Cloudot.Infrastructure.Database;
using Cloudot.Infrastructure.Messaging;
using Cloudot.Infrastructure.Redis;
using Cloudot.Module.Management.Application;
using Cloudot.Module.Management.Infrastructure;
using Cloudot.Shared;
using Cloudot.Shared.EntityFramework;
using Cloudot.WebAPI.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        Assembly[] validatorAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && a.GetTypes().Any(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.BaseType is { } bt &&
                bt.IsGenericType &&
                bt.GetGenericTypeDefinition() == typeof(AbstractValidator<>)))
            .ToArray();

        fv.RegisterValidatorsFromAssemblies(validatorAssemblies);
        fv.LocalizationEnabled = false;
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCloudotShared();
builder.Services.AddEntityFrameworkShared();
builder.Services.AddRedisCacheManager(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddDatabaseShared(builder.Configuration);
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

// Localization middleware — Accept-Language header'a göre kültürü ayarlar
IOptions<RequestLocalizationOptions> locOptions =
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

// HTTPS yönlendirmesi
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Controller endpointleri
app.MapControllers();

// Global exception middleware (en sona yakın)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Uygulama çalıştır
app.Run();