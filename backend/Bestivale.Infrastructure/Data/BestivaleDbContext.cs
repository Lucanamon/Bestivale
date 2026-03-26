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
    public DbSet<Egg> Eggs => Set<Egg>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryEgg> InventoryEggs => Set<InventoryEgg>();
    public DbSet<InventoryMonster> InventoryMonsters => Set<InventoryMonster>();

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
            entity.Property(m => m.IsFavorite).IsRequired().HasDefaultValue(false);
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

            entity.Property(l => l.EggId);
            entity.Property(l => l.InventoryItemId);

            entity.HasOne(l => l.Monster)
                .WithMany()
                .HasForeignKey(l => l.MonsterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.SellerUser)
                .WithMany()
                .HasForeignKey(l => l.SellerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Egg)
                .WithMany()
                .HasForeignKey(l => l.EggId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(l => l.InventoryItem)
                .WithMany()
                .HasForeignKey(l => l.InventoryItemId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(l => l.Status);
            entity.HasIndex(l => l.CreatedAt);
            entity.HasIndex(l => new { l.Status, l.Price });
        });

        modelBuilder.Entity<Egg>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TemplateCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ColorHex).IsRequired().HasMaxLength(16);
            entity.Property(e => e.ColorDescription).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsListed).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.IsFavorite).IsRequired().HasDefaultValue(false);

            entity.HasOne(e => e.OwnerUser)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OwnerUserId);
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.ItemType).IsRequired();
            entity.Property(i => i.IsFavorite).IsRequired().HasDefaultValue(false);
            entity.Property(i => i.IsListed).IsRequired().HasDefaultValue(false);
            entity.Property(i => i.CreatedAt).IsRequired();

            entity.HasOne(i => i.OwnerUser)
                .WithMany()
                .HasForeignKey(i => i.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(i => new { i.OwnerUserId, i.CreatedAt });
            entity.HasIndex(i => new { i.OwnerUserId, i.IsFavorite });
            entity.HasIndex(i => new { i.OwnerUserId, i.ItemType });
        });

        modelBuilder.Entity<InventoryEgg>(entity =>
        {
            entity.HasKey(e => e.InventoryItemId);

            entity.Property(e => e.TemplateCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ColorHex).IsRequired().HasMaxLength(16);
            entity.Property(e => e.ColorDescription).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.InventoryItem)
                .WithOne(i => i.InventoryEgg)
                .HasForeignKey<InventoryEgg>(e => e.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InventoryMonster>(entity =>
        {
            entity.HasKey(m => m.InventoryItemId);

            entity.HasOne(m => m.InventoryItem)
                .WithOne(i => i.InventoryMonster)
                .HasForeignKey<InventoryMonster>(m => m.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Monster)
                .WithMany()
                .HasForeignKey(m => m.MonsterId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

