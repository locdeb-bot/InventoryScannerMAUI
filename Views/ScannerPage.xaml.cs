using Microsoft.Maui.Controls;
using InventoryScannerMAUI.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

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

        CameraView.BarcodesDetected += OnBarcodesDetected;
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

    private void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        Dispatcher.Dispatch(async () =>
        {
            var barcode = e.Results?.FirstOrDefault();
            if (barcode != null)
            {
                await _viewModel.OnBarcodeDetected(barcode.Value);
            }
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
