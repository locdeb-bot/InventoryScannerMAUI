using Microsoft.Maui.Controls;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.Views;

public partial class ItemDetailPage : ContentPage
{
    private readonly InventoryItem _item;
    private readonly Action<InventoryItem> _onSave;

    public ItemDetailPage(InventoryItem item, Action<InventoryItem> onSave)
    {
        InitializeComponent();
        _item = item;
        _onSave = onSave;

        NameEntry.Text = item.Name;
        SKUEntry.Text = item.SKU;
        BarcodeEntry.Text = item.Barcode;
        QuantityEntry.Text = item.Quantity.ToString();
        LocationEntry.Text = item.Location;
        DescriptionEntry.Text = item.Description;
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        _item.Name = NameEntry.Text ?? string.Empty;
        _item.SKU = SKUEntry.Text ?? string.Empty;
        _item.Barcode = BarcodeEntry.Text ?? string.Empty;

        if (int.TryParse(QuantityEntry.Text, out int qty))
        {
            _item.Quantity = qty;
        }

        _item.Location = LocationEntry.Text;
        _item.Description = DescriptionEntry.Text;
        _item.LastUpdated = DateTime.UtcNow;
        _item.SyncStatus = SyncStatus.PendingSync;

        _onSave?.Invoke(_item);
        Navigation.PopModalAsync();
    }
}
