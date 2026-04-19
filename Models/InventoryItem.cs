using System.ComponentModel.DataAnnotations;

namespace InventoryScannerMAUI.Models;

public enum SyncStatus
{
    Synced = 0,
    PendingSync = 1,
    Failed = 2
}

public class InventoryItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public SyncStatus SyncStatus { get; set; } = SyncStatus.Synced;
}
