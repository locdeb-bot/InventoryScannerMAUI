using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using InventoryScannerMAUI.Data;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.ViewModels;

public partial class ScannerViewModel : ViewModelBase
{
    private readonly InventoryDbContext _dbContext;

    [ObservableProperty]
    private string _scannedBarcode = string.Empty;

    [ObservableProperty]
    private InventoryItem? _foundItem;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusMessage = "Point camera at a barcode to scan";

    [ObservableProperty]
    private bool _showItemDetails;

    [ObservableProperty]
    private bool _showNewItemForm;

    public ScannerViewModel()
    {
        _dbContext = new InventoryDbContext();
    }

    public async Task InitializeAsync()
    {
        await _dbContext.EnsureDatabaseCreatedAsync();
    }

    [RelayCommand]
    public async Task OnBarcodeDetected(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode)) return;

        ScannedBarcode = barcode;
        StatusMessage = $"Scanning: {barcode}";

        try
        {
            var item = await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.Barcode == barcode);

            if (item != null)
            {
                FoundItem = item;
                ShowItemDetails = true;
                ShowNewItemForm = false;
                StatusMessage = $"Found: {item.Name}";
            }
            else
            {
                FoundItem = new InventoryItem { Barcode = barcode };
                ShowItemDetails = false;
                ShowNewItemForm = true;
                StatusMessage = "New item - Enter details";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveItem()
    {
        if (FoundItem == null) return;

        try
        {
            if (FoundItem.Id == 0)
            {
                FoundItem.CreatedAt = DateTime.UtcNow;
                FoundItem.LastUpdated = DateTime.UtcNow;
                FoundItem.SyncStatus = Models.SyncStatus.PendingSync;
                _dbContext.InventoryItems.Add(FoundItem);
            }
            else
            {
                FoundItem.LastUpdated = DateTime.UtcNow;
                FoundItem.SyncStatus = Models.SyncStatus.PendingSync;
                _dbContext.InventoryItems.Update(FoundItem);
            }

            await _dbContext.SaveChangesAsync();
            StatusMessage = "Item saved successfully!";
            ClearForm();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task IncrementQuantity()
    {
        if (FoundItem != null && FoundItem.Id > 0)
        {
            FoundItem.Quantity++;
            FoundItem.LastUpdated = DateTime.UtcNow;
            FoundItem.SyncStatus = Models.SyncStatus.PendingSync;
            _dbContext.InventoryItems.Update(FoundItem);
            await _dbContext.SaveChangesAsync();
            OnPropertyChanged(nameof(FoundItem));
        }
    }

    [RelayCommand]
    private async Task DecrementQuantity()
    {
        if (FoundItem != null && FoundItem.Id > 0 && FoundItem.Quantity > 0)
        {
            FoundItem.Quantity--;
            FoundItem.LastUpdated = DateTime.UtcNow;
            FoundItem.SyncStatus = Models.SyncStatus.PendingSync;
            _dbContext.InventoryItems.Update(FoundItem);
            await _dbContext.SaveChangesAsync();
            OnPropertyChanged(nameof(FoundItem));
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        FoundItem = null;
        ScannedBarcode = string.Empty;
        ShowItemDetails = false;
        ShowNewItemForm = false;
        StatusMessage = "Point camera at a barcode to scan";
    }

    [RelayCommand]
    private void StartScanning()
    {
        IsScanning = true;
        StatusMessage = "Scanning...";
    }

    [RelayCommand]
    private void StopScanning()
    {
        IsScanning = false;
        StatusMessage = "Point camera at a barcode to scan";
    }
}
