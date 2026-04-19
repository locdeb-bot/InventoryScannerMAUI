using Microsoft.Maui.Controls;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace InventoryScannerMAUI.Views;

public partial class ConnectionStringScannerPage : ContentPage
{
    private bool _isProcessing;
    private string _lastScannedData = string.Empty;

    public event EventHandler<string?>? ConnectionStringScanned;
    public string? ScannedConnectionString { get; private set; }

    public ConnectionStringScannerPage()
    {
        InitializeComponent();
        CameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.QrCode,
            AutoRotate = true,
            Multiple = false
        };
    }

    private void CameraView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;

        var result = e.Results?.FirstOrDefault();
        if (result == null || string.IsNullOrEmpty(result.Value)) return;
        if (result.Value == _lastScannedData) return;

        _isProcessing = true;
        _lastScannedData = result.Value;

        Dispatcher.Dispatch(() =>
        {
            ScannedDataLabel.Text = result.Value;
            UseConnectionStringButton.IsEnabled = true;
        });
    }

    private async void UseConnectionStringButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_lastScannedData))
        {
            await DisplayAlert("Error", "No connection string scanned", "OK");
            return;
        }

        // Validate that it looks like a connection string
        var scannedValue = _lastScannedData.Trim();

        if (!IsValidConnectionString(scannedValue))
        {
            bool useAnyway = await DisplayAlert(
                "Warning",
                "The scanned data may not be a valid SQL Server connection string. It should contain keywords like 'Server=', 'Database=', 'User Id=', or 'Password='.\n\nDo you want to use it anyway?",
                "Use Anyway",
                "Cancel");

            if (!useAnyway) return;
        }

        ScannedConnectionString = scannedValue;
        ConnectionStringScanned?.Invoke(this, ScannedConnectionString);
        await Navigation.PopAsync();
    }

    private async void ScanAgainButton_Clicked(object sender, EventArgs e)
    {
        _isProcessing = false;
        _lastScannedData = string.Empty;
        ScannedDataLabel.Text = "Waiting for scan...";
        UseConnectionStringButton.IsEnabled = false;
        await DisplayAlert("Ready", "Point camera at a QR code to scan again", "OK");
    }

    private bool IsValidConnectionString(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        // Check for common SQL Server connection string keywords
        var keywords = new[] { "server=", "data source=", "database=", "initial catalog=",
                               "user id=", "uid=", "password=", "pwd=", "integrated security=" };

        var lowerValue = value.ToLowerInvariant();
        return keywords.Any(k => lowerValue.Contains(k));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CameraView.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CameraView.IsDetecting = false;
    }
}
