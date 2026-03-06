using Bestivale.Application.Interfaces;
using Bestivale.Infrastructure.Data;
using Bestivale.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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
builder.Services.AddDbContext<BestivaleDbContext>(options =>
    options.UseInMemoryDatabase("Bestivale"));

// Services
builder.Services.AddScoped<IMonsterService, MonsterService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bestivale API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
