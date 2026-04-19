using Microsoft.Maui.Controls;
using InventoryScannerMAUI.ViewModels;
using ZXing.Net.Maui;

namespace InventoryScannerMAUI.Views;

public partial class ScannerPage : ContentPage
{
    private readonly ScannerViewModel _viewModel;

    public ScannerPage()
    {
        InitializeComponent();
        _viewModel = new ScannerViewModel();
        BindingContext = _viewModel;

        Loaded += async (s, e) =>
        {
            await _viewModel.InitializeAsync();
            SetupCamera();
        };

        CameraView.BarcodeDetected += OnBarcodeDetected;
    }

    private void SetupCamera()
    {
        CameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            TryHarder = true
        };
    }

    private void OnBarcodeDetected(object? sender, BarcodeDetectedEventArgs e)
    {
        Dispatcher.Dispatch(async () =>
        {
            await _viewModel.OnBarcodeDetectedCommand.ExecuteAsync(e.Value);
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CameraView.IsDetecting = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CameraView.IsDetecting = true;
    }
}
