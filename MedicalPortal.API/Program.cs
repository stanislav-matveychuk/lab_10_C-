using MedicalPortal.Application;
using MedicalPortal.Domain;
using MedicalPortal.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(
    $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "medical.db")}"));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPortalService, PortalService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
app.Use(async (context, next) =>
{
    try { await next(context); }
    catch (Exception e) when (e is ArgumentException or KeyNotFoundException or InvalidOperationException or DbUpdateException)
    {
        context.Response.StatusCode = e switch
        {
            KeyNotFoundException => 404,
            ArgumentException => 400,
            _ => 409
        };
        await context.Response.WriteAsJsonAsync(new { error = e is DbUpdateException ? "Конфлікт збереження даних." : e.Message });
    }
});
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();
