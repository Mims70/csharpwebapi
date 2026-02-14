using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration - supports both SQLite (local) and PostgreSQL (production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? "Data Source=products.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString.StartsWith("postgres"))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

var app = builder.Build();

// Auto-run migrations on startup (for production)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();