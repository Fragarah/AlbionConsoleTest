using Microsoft.EntityFrameworkCore;
using AlbionConsole.Models;

namespace AlbionConsole.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<TrackedItem> TrackedItems => Set<TrackedItem>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.UniqueName).IsRequired();
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<TrackedItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Item)
                .WithMany(i => i.TrackedItems)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BuyPriceMax).IsRequired();
            entity.Property(e => e.BuyPriceMin).IsRequired();
            entity.Property(e => e.SellPriceMax).IsRequired();
            entity.Property(e => e.SellPriceMin).IsRequired();
            entity.Property(e => e.Location).IsRequired();
            entity.Property(e => e.Quality).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.HasOne(e => e.Item)
                .WithMany(i => i.PriceHistories)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 