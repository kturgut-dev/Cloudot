using Cloudot.Shared;

var builder = WebApplication.CreateBuilder(args);

// Servis kayıtları
builder.Services.AddControllers();
builder.Services.AddCloudotShared();
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