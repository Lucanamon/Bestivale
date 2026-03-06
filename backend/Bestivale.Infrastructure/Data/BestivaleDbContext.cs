using Bestivale.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Data;

public sealed class BestivaleDbContext : DbContext
{
    public BestivaleDbContext(DbContextOptions<BestivaleDbContext> options) : base(options)
    {
    }

    public DbSet<Monster> Monsters => Set<Monster>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Monster>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Mythology).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Description).IsRequired().HasMaxLength(4000);
            entity.Property(m => m.ImageUrl).IsRequired().HasMaxLength(1000);
        });
    }
}

