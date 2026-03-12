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

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BestivaleDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        // Development: in-memory database
        options.UseInMemoryDatabase("Bestivale");
    }
    else
    {
        // Production-ready: PostgreSQL
        options.UseNpgsql(connectionString);
    }
});

// Services
builder.Services.AddScoped<IMonsterRepository, MonsterRepository>();
builder.Services.AddScoped<IMonsterService, MonsterService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, AuthService>();
builder.Services.AddScoped<IMarketRepository, MarketRepository>();
builder.Services.AddScoped<IMarketService, MarketService>();
builder.Services.AddScoped<IEggRepository, EggRepository>();
builder.Services.AddScoped<EggService>();

var app = builder.Build();

// Ensure each user has at least 5 eggs on startup
using (var scope = app.Services.CreateScope())
{
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

app.UseHttpsRedirection();

app.UseCors("DevCors");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
