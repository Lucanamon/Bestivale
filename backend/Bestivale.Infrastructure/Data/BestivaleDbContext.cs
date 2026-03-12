using Bestivale.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Data;

public sealed class BestivaleDbContext : DbContext
{
    private static readonly Guid RootAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public BestivaleDbContext(DbContextOptions<BestivaleDbContext> options) : base(options)
    {
    }

    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<User> Users => Set<User>();
    public DbSet<MarketListing> MarketListings => Set<MarketListing>();

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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
            entity.Property(u => u.CurrencyBalance).IsRequired().HasDefaultValue(10);
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.IsRootAdmin).IsRequired().HasDefaultValue(false);

            entity.HasIndex(u => u.Username).IsUnique();

            // RootAdmin seed user
            var rootAdmin = new User
            {
                Id = RootAdminId,
                Username = "rootadmin",
                Role = "RootAdmin",
                CurrencyBalance = 9999,
                CreatedAt = DateTime.UtcNow,
                IsRootAdmin = true
            };

            rootAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("guardianOP");

            entity.HasData(rootAdmin);
        });

        modelBuilder.Entity<MarketListing>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Price).IsRequired();
            entity.Property(l => l.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(l => l.CreatedAt).IsRequired();

            entity.HasOne(l => l.Monster)
                .WithMany()
                .HasForeignKey(l => l.MonsterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.SellerUser)
                .WithMany()
                .HasForeignKey(l => l.SellerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(l => l.Status);
            entity.HasIndex(l => l.CreatedAt);
            entity.HasIndex(l => new { l.Status, l.Price });
        });
    }
}

