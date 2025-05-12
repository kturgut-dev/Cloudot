using System.Net;
using System.Text.Json;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Results;
using IResult = Cloudot.Shared.Results.IResult;

namespace Cloudot.WebAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        context.Response.ContentType = "application/json";

        try
        {
            await _next(context);
        }
        catch (BaseAppException appEx)
        {
            _logger.LogWarning(appEx, "Uygulama hatası yakalandı");

            context.Response.StatusCode = appEx.StatusCode;

            IResult result = Result.Fail(appEx.Message, appEx.StatusCode);
            string json = JsonSerializer.Serialize(result, options);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sistemsel hata yakalandı");

            context.Response.StatusCode = 500;

            IResult result = Result.Fail("Beklenmeyen bir hata oluştu.", 500);
            string json = JsonSerializer.Serialize(result, options);
            await context.Response.WriteAsync(json);
        }
    }
}