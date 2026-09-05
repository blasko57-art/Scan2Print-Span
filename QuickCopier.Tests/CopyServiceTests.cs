using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;
using QuickCopier.Core.Services;

namespace QuickCopier.Tests;

public class CopyServiceTests
{
    [Fact]
    public async Task CopyAsync_ScansThenPrints()
    {
        // Arrange
        var scannerService = new FakeScannerService();
        var printerService = new FakePrinterService();

        var copyService = new CopyService(
            scannerService,
            printerService);

        var scanner = CreateScanner();
        var printer = CreatePrinter();

        // Act
        await copyService.CopyAsync(
            scanner,
            printer,
            new ScanSettings(),
            new PrintSettings());

        // Assert
        Assert.True(scannerService.ScanWasCalled);
        Assert.True(printerService.PrintWasCalled);
    }

    [Fact]
    public async Task CopyAsync_WhenScanningFails_DoesNotPrint()
    {
        // Arrange
        var scannerService = new FailingScannerService();
        var printerService = new FakePrinterService();

        var copyService = new CopyService(
            scannerService,
            printerService);

        var scanner = CreateScanner();
        var printer = CreatePrinter();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            copyService.CopyAsync(
                scanner,
                printer,
                new ScanSettings(),
                new PrintSettings()));

        Assert.False(printerService.PrintWasCalled);
    }

    [Fact]
    public async Task CopyAsync_AfterPrinting_DisposesScannedDocument()
    {
        // Arrange
        var scannerService = new TrackingScannerService();
        var printerService = new FakePrinterService();

        var copyService = new CopyService(
            scannerService,
            printerService);

        var scanner = CreateScanner();
        var printer = CreatePrinter();

        // Act
        await copyService.CopyAsync(
            scanner,
            printer,
            new ScanSettings(),
            new PrintSettings());

        // Assert
        Assert.NotNull(scannerService.Document);

        Assert.Throws<ObjectDisposedException>(
            () => scannerService.Document!.Data.ReadByte());
    }

    [Fact]
    public async Task CopyAsync_WhenPrintingFails_DisposesScannedDocument()
    {
        // Arrange
        var scannerService = new TrackingScannerService();
        var printerService = new FailingPrinterService();

        var copyService = new CopyService(
            scannerService,
            printerService);

        var scanner = CreateScanner();
        var printer = CreatePrinter();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            copyService.CopyAsync(
                scanner,
                printer,
                new ScanSettings(),
                new PrintSettings()));

        // The document must still be disposed
        // even though printing failed.
        Assert.NotNull(scannerService.Document);

        Assert.Throws<ObjectDisposedException>(
            () => scannerService.Document!.Data.ReadByte());
    }

    [Fact]
    public async Task CopyAsync_WhenScannerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var copyService = new CopyService(
            new FakeScannerService(),
            new FakePrinterService());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            copyService.CopyAsync(
                null!,
                CreatePrinter(),
                new ScanSettings(),
                new PrintSettings()));
    }

    [Fact]
    public async Task CopyAsync_WhenPrinterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var copyService = new CopyService(
            new FakeScannerService(),
            new FakePrinterService());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            copyService.CopyAsync(
                CreateScanner(),
                null!,
                new ScanSettings(),
                new PrintSettings()));
    }

    [Fact]
    public async Task CopyAsync_WhenScanSettingsAreNull_ThrowsArgumentNullException()
    {
        // Arrange
        var copyService = new CopyService(
            new FakeScannerService(),
            new FakePrinterService());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            copyService.CopyAsync(
                CreateScanner(),
                CreatePrinter(),
                null!,
                new PrintSettings()));
    }

    [Fact]
    public async Task CopyAsync_WhenPrintSettingsAreNull_ThrowsArgumentNullException()
    {
        // Arrange
        var copyService = new CopyService(
            new FakeScannerService(),
            new FakePrinterService());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            copyService.CopyAsync(
                CreateScanner(),
                CreatePrinter(),
                new ScanSettings(),
                null!));
    }

    private static ScannerDevice CreateScanner()
    {
        return new ScannerDevice
        {
            Id = "scanner-1",
            Name = "Test Scanner"
        };
    }

    private static PrinterDevice CreatePrinter()
    {
        return new PrinterDevice
        {
            Id = "printer-1",
            Name = "Test Printer"
        };
    }

    private sealed class FakeScannerService : IScannerService
    {
        public bool ScanWasCalled { get; private set; }

        public Task<IReadOnlyList<ScannerDevice>> GetScannersAsync()
        {
            return Task.FromResult<IReadOnlyList<ScannerDevice>>(
                Array.Empty<ScannerDevice>());
        }

        public Task<ScannedDocument> ScanAsync(
            ScannerDevice scanner,
            ScanSettings settings)
        {
            ScanWasCalled = true;

            var data = new MemoryStream(
                new byte[] { 1, 2, 3 });

            return Task.FromResult(
                new ScannedDocument(data));
        }
    }

    private sealed class TrackingScannerService : IScannerService
    {
        public ScannedDocument? Document { get; private set; }

        public Task<IReadOnlyList<ScannerDevice>> GetScannersAsync()
        {
            return Task.FromResult<IReadOnlyList<ScannerDevice>>(
                Array.Empty<ScannerDevice>());
        }

        public Task<ScannedDocument> ScanAsync(
            ScannerDevice scanner,
            ScanSettings settings)
        {
            Document = new ScannedDocument(
                new MemoryStream(
                    new byte[] { 1, 2, 3 }));

            return Task.FromResult(Document);
        }
    }

    private sealed class FailingScannerService : IScannerService
    {
        public Task<IReadOnlyList<ScannerDevice>> GetScannersAsync()
        {
            return Task.FromResult<IReadOnlyList<ScannerDevice>>(
                Array.Empty<ScannerDevice>());
        }

        public Task<ScannedDocument> ScanAsync(
            ScannerDevice scanner,
            ScanSettings settings)
        {
            throw new InvalidOperationException(
                "Test scanner failure.");
        }
    }

    private sealed class FakePrinterService : IPrinterService
    {
        public bool PrintWasCalled { get; private set; }

        public IReadOnlyList<PrinterDevice> GetPrinters()
        {
            return Array.Empty<PrinterDevice>();
        }

        public Task PrintAsync(
            PrinterDevice printer,
            ScannedDocument document,
            PrintSettings settings)
        {
            PrintWasCalled = true;

            return Task.CompletedTask;
        }
    }

    private sealed class FailingPrinterService : IPrinterService
    {
        public IReadOnlyList<PrinterDevice> GetPrinters()
        {
            return Array.Empty<PrinterDevice>();
        }

        public Task PrintAsync(
            PrinterDevice printer,
            ScannedDocument document,
            PrintSettings settings)
        {
            throw new InvalidOperationException(
                "Test printer failure.");
        }
    }
}
