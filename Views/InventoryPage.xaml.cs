using Microsoft.Maui.Controls;
using InventoryScannerMAUI.ViewModels;
using InventoryScannerMAUI.Models;

namespace InventoryScannerMAUI.Views;

public partial class InventoryPage : ContentPage
{
    private readonly InventoryViewModel _viewModel;

    public InventoryPage()
    {
        InitializeComponent();
        _viewModel = new InventoryViewModel();
        BindingContext = _viewModel;

        Loaded += async (s, e) =>
        {
            await _viewModel.InitializeAsync();
        };
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is InventoryItem item)
        {
            _viewModel.SelectedItem = item;
            var detailPage = new ItemDetailPage(item, async (updatedItem) =>
            {
                await _viewModel.SaveItemCommand.ExecuteAsync(null);
            });
            await Navigation.PushModalAsync(detailPage);
        }
    }
}
