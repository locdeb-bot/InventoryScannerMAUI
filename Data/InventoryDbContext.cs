using Microsoft.EntityFrameworkCore;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.Data;

public class InventoryDbContext : DbContext
{
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;

    private readonly string _dbPath;

    public InventoryDbContext()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appData, "InventoryScannerMAUI");
        Directory.CreateDirectory(appFolder);
        _dbPath = Path.Combine(appFolder, "inventory.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Barcode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Quantity).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.Barcode);
        });

        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem
            {
                Id = 1,
                Name = "Sample Product 1",
                SKU = "SKU-001",
                Barcode = "1234567890123",
                Quantity = 100,
                Description = "A sample product for testing",
                Location = "Warehouse A",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                SyncStatus = SyncStatus.Synced
            },
            new InventoryItem
            {
                Id = 2,
                Name = "Sample Product 2",
                SKU = "SKU-002",
                Barcode = "2345678901234",
                Quantity = 50,
                Description = "Another sample product",
                Location = "Warehouse B",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                SyncStatus = SyncStatus.Synced
            }
        );
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }
}
