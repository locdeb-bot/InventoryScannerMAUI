using Microsoft.Maui.Controls;
using InventoryScannerMAUI.ViewModels;

namespace InventoryScannerMAUI.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = new SettingsViewModel();
        BindingContext = _viewModel;

        Loaded += async (s, e) =>
        {
            _viewModel.SetNavigation(Navigation);
            await _viewModel.InitializeAsync();
        };
    }
}
