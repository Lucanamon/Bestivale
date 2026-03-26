using Bestivale.Application.Interfaces;
using Bestivale.Application.Services;
using Bestivale.Infrastructure.Data;
using Bestivale.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// CORS (dev): allow Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "http://localhost:4201",
                "https://localhost:4201",
                "http://localhost:4301",
                "https://localhost:4301",
                "http://localhost:4401",
                "https://localhost:4401"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bestivale API",
        Version = "v1",
        Description = "Bestivale Monster Encyclopedia API"
    });
});

// Database: always use PostgreSQL (dev & prod)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<BestivaleDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Services
builder.Services.AddScoped<IMonsterRepository, MonsterRepository>();
builder.Services.AddScoped<IMonsterService, MonsterService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, AuthService>();
builder.Services.AddScoped<IMarketRepository, MarketRepository>();
builder.Services.AddScoped<IMarketService, MarketService>();
builder.Services.AddScoped<IEggRepository, EggRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<EggService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<AdminInventoryService>();

var app = builder.Build();

// Ensure base content & starter inventory on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BestivaleDbContext>();
    await db.Database.MigrateAsync();
    Console.WriteLine("Migrate database successfully.");

    // Register the Egg as a "Monster" so it can be listed on the Market
    var eggMonsterExists = await db.Monsters.AnyAsync(m => m.Name == "Embryo mutagen egg");
    if (!eggMonsterExists)
    {
        db.Monsters.Add(new Bestivale.Domain.Entities.Monster
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Embryo mutagen egg",
            Mythology = "Egg",
            Description = "A humming egg infused with embryo mutagen. Its shell holds an unknown future.",
            ImageUrl = "https://placehold.co/600x400/png?text=Egg"
        });
        await db.SaveChangesAsync();
    }

    var eggService = scope.ServiceProvider.GetRequiredService<EggService>();
    await eggService.EnsureAtLeastEggsForAllUsersAsync(5);
}

// Configure the HTTP request pipeline
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bestivale API v1");
    options.RoutePrefix = "swagger";
});

// In local dev we often run HTTP-only profiles; avoid noisy redirect warnings.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("DevCors");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();