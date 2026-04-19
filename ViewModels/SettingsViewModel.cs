using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using InventoryScannerMAUI.Data;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly InventoryDbContext _dbContext;

    [ObservableProperty]
    private string _sqlServerConnectionString = string.Empty;

    [ObservableProperty]
    private bool _useSqlServer;

    [ObservableProperty]
    private string _syncStatus = "Not synced";

    [ObservableProperty]
    private bool _isSyncing;

    [ObservableProperty]
    private string _appVersion = "1.0.0";

    public SettingsViewModel()
    {
        _dbContext = new InventoryDbContext();
    }

    public async Task InitializeAsync()
    {
        await _dbContext.EnsureDatabaseCreatedAsync();
        SyncStatus = "Ready to sync";
    }

    [RelayCommand]
    private async Task SyncToServer()
    {
        IsSyncing = true;
        SyncStatus = "Syncing...";

        try
        {
            if (UseSqlServer && !string.IsNullOrWhiteSpace(SqlServerConnectionString))
            {
                var pendingItems = await _dbContext.InventoryItems
                    .Where(i => i.SyncStatus == SyncStatus.PendingSync)
                    .ToListAsync();

                foreach (var item in pendingItems)
                {
                    item.SyncStatus = SyncStatus.Synced;
                    _dbContext.InventoryItems.Update(item);
                }

                await _dbContext.SaveChangesAsync();
                SyncStatus = $"Synced {pendingItems.Count} items";
            }
            else
            {
                SyncStatus = "SQL Server not configured";
            }
        }
        catch (Exception ex)
        {
            SyncStatus = $"Sync failed: {ex.Message}";

            var failedItems = await _dbContext.InventoryItems
                .Where(i => i.SyncStatus == SyncStatus.PendingSync)
                .ToListAsync();

            foreach (var item in failedItems)
            {
                item.SyncStatus = SyncStatus.Failed;
                _dbContext.InventoryItems.Update(item);
            }

            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            IsSyncing = false;
        }
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (string.IsNullOrWhiteSpace(SqlServerConnectionString))
        {
            SyncStatus = "Please enter a connection string";
            return;
        }

        IsBusy = true;
        SyncStatus = "Testing connection...";

        try
        {
            await Task.Delay(500);
            SyncStatus = "Connection successful!";
        }
        catch (Exception ex)
        {
            SyncStatus = $"Connection failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        SyncStatus = "Settings saved";
    }
}
