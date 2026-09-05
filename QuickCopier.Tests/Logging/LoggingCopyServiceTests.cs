using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Logging;
using QuickCopier.Core.Models;

namespace QuickCopier.Tests.Logging;

public class LoggingCopyServiceTests
{
    [Fact]
    public async Task CopyAsync_LogsSuccessfulCopy()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var logger =
            loggerFactory.CreateLogger<LoggingCopyService>();

        var inner = new FakeCopyService();

        var service = new LoggingCopyService(
            inner,
            logger);

        // Act
        await service.CopyAsync(
            CreateScanner(),
            CreatePrinter(),
            new ScanSettings(),
            new PrintSettings());

        // Assert
        Assert.True(inner.CopyWasCalled);

        Assert.Equal(2, provider.Entries.Count);

        Assert.Contains(
            provider.Entries,
            entry =>
                entry.Level == LogLevel.Information &&
                entry.Message.StartsWith("Copy started."));

        Assert.Contains(
            provider.Entries,
            entry =>
                entry.Level == LogLevel.Information &&
                entry.Message ==
                    "Copy completed successfully. Scanner: Test Scanner, Printer: Test Printer");

    }

    [Fact]
    public async Task CopyAsync_WhenInnerServiceFails_LogsErrorAndRethrows()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var logger =
            loggerFactory.CreateLogger<LoggingCopyService>();

        var inner = new FailingCopyService();

        var service = new LoggingCopyService(
            inner,
            logger);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () =>
                service.CopyAsync(
                    CreateScanner(),
                    CreatePrinter(),
                    new ScanSettings(),
                    new PrintSettings()));

        var errorEntry = Assert.Single(
            provider.Entries,
            entry => entry.Level == LogLevel.Error);

        Assert.Equal(
            "Copy failed. Scanner: Test Scanner, Printer: Test Printer",
            errorEntry.Message);

        Assert.NotNull(errorEntry.Exception);
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

    private sealed class FakeCopyService : ICopyService
    {
        public bool CopyWasCalled { get; private set; }

        public Task CopyAsync(
            ScannerDevice scanner,
            PrinterDevice printer,
            ScanSettings scanSettings,
            PrintSettings printSettings)
        {
            CopyWasCalled = true;

            return Task.CompletedTask;
        }
    }

    private sealed class FailingCopyService : ICopyService
    {
        public Task CopyAsync(
            ScannerDevice scanner,
            PrinterDevice printer,
            ScanSettings scanSettings,
            PrintSettings printSettings)
        {
            throw new InvalidOperationException(
                "Test copy failure.");
        }
    }
}