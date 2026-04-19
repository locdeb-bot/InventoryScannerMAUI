using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using InventoryScannerMAUI.Data;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.ViewModels;

public partial class InventoryViewModel : ViewModelBase
{
    private readonly InventoryDbContext _dbContext;

    [ObservableProperty]
    private ObservableCollection<InventoryItem> _items = new();

    [ObservableProperty]
    private InventoryItem? _selectedItem;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isRefreshing;

    private List<InventoryItem> _allItems = new();

    public InventoryViewModel()
    {
        _dbContext = new InventoryDbContext();
    }

    public async Task InitializeAsync()
    {
        await _dbContext.EnsureDatabaseCreatedAsync();
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsBusy = true;
        try
        {
            _allItems = await _dbContext.InventoryItems
                .OrderBy(i => i.Name)
                .ToListAsync();
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadItemsAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task DeleteItem(InventoryItem item)
    {
        if (item == null) return;

        try
        {
            _dbContext.InventoryItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            _allItems.Remove(item);
            ApplyFilter();
        }
        catch (Exception)
        {
            // Handle error
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allItems
            : _allItems.Where(i =>
                i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                i.SKU.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                i.Barcode.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

        Items.Clear();
        foreach (var item in filtered)
        {
            Items.Add(item);
        }
    }

    [RelayCommand]
    private async Task SaveItem()
    {
        if (SelectedItem == null) return;

        try
        {
            if (SelectedItem.Id == 0)
            {
                SelectedItem.CreatedAt = DateTime.UtcNow;
                SelectedItem.SyncStatus = SyncStatus.PendingSync;
                _dbContext.InventoryItems.Add(SelectedItem);
            }
            else
            {
                SelectedItem.LastUpdated = DateTime.UtcNow;
                SelectedItem.SyncStatus = SyncStatus.PendingSync;
                _dbContext.InventoryItems.Update(SelectedItem);
            }

            await _dbContext.SaveChangesAsync();
            await LoadItemsAsync();
        }
        catch (Exception)
        {
            // Handle error
        }
    }
}
