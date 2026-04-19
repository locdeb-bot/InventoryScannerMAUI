# Inventory Scanner - MAUI iOS Application

A cross-platform inventory management mobile application built with .NET MAUI and .NET 10, featuring barcode scanning capabilities and Microsoft SQL database integration.

## Features

- **Barcode Scanning**: Real-time camera barcode scanning supporting QR codes, Code128, Code39, EAN-13, and UPC-A formats
- **Inventory Management**: View, add, edit, and delete inventory items
- **SQL Database Integration**: Local SQLite storage with optional Microsoft SQL Server sync
- **Connection String Scanning**: Scan QR codes containing SQL Server connection strings for easy setup
- **Offline Support**: Works offline with local database, syncs when connected
- **Modern UI**: iOS Human Interface Guidelines compliant design with dark mode support

## Technology Stack

- **Framework**: .NET 10 with MAUI (Multi-platform App UI)
- **Language**: C# 13
- **Database**:
  - Local: SQLite via Entity Framework Core
  - Remote: Microsoft SQL Server (Azure SQL compatible)
- **Barcode Scanning**: ZXing.Net.MAUI
- **Architecture**: MVVM with CommunityToolkit.Mvvm
- **Target Platform**: iOS 16.0+

## Project Structure

```
InventoryScannerMAUI/
├── Models/              # Data models (InventoryItem)
├── Data/                # Database context (EF Core)
├── ViewModels/          # MVVM ViewModels
├── Views/               # MAUI Pages
├── Converters/          # Value converters
├── Resources/           # Styles and themes
├── Platforms/           # Platform-specific code
│   └── iOS/            # iOS configuration
└── App.xaml            # Application entry point
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 (Windows) or Visual Studio Code (macOS)
- For iOS: macOS with Xcode 15+ and Apple Developer account

### Build Instructions

1. Clone the repository
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Build for iOS simulator:
   ```bash
   dotnet build -f net10.0-ios -c Debug
   ```
4. Run on iOS Simulator:
   ```bash
   dotnet run -f net10.0-ios -c Debug
   ```

### Configuration

#### SQL Server Connection

To configure SQL Server sync, go to Settings tab and:
1. Enable "Use SQL Server" toggle
2. Enter your SQL Server connection string (or scan a QR code)
3. Test the connection
4. Use "Sync to Server" to sync pending changes

**Option: Scan Connection String via QR Code**
Instead of manually typing the connection string, you can:
1. Click "Scan Connection String (QR Code)"
2. Point the camera at a QR code containing your SQL Server connection string
3. The scanned value will be automatically filled in

**Creating a QR Code for your connection string:**
Many online QR code generators support text. Simply encode your connection string as plain text:
```
Server=myserver.database.windows.net;Database=inventory;User Id=user;Password=password;
```

Example connection string:
```
Server=myserver.database.windows.net;Database=inventory;User Id=user;Password=password;
```

## Data Model

### InventoryItem
- `Id`: Auto-increment primary key
- `Name`: Item name (required, max 200 chars)
- `SKU`: Stock keeping unit (required, unique, max 50 chars)
- `Barcode`: Product barcode (required, max 100 chars)
- `Quantity`: Current stock quantity
- `Description`: Optional item description (max 1000 chars)
- `Location`: Warehouse/storage location (max 100 chars)
- `CreatedAt`: Creation timestamp
- `LastUpdated`: Last modification timestamp
- `SyncStatus`: Sync state (Synced/PendingSync/Failed)

## Screenshots

The app features three main tabs:
1. **Scanner**: Camera-based barcode scanning with instant lookup
2. **Inventory**: Searchable list of all inventory items
3. **Settings**: Database configuration and sync controls

## License

MIT License

## Author

InventoryScannerMAUI Team
