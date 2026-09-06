using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Logging;
using QuickCopier.Core.Models;

namespace QuickCopier.Tests.Logging;

public class LoggingPrinterServiceTests
{
    [Fact]
    public void GetPrinters_WhenSuccessful_LogsStartAndCompletion()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FakePrinterService();

        var logger =
            loggerFactory.CreateLogger<LoggingPrinterService>();

        var service =
            new LoggingPrinterService(inner, logger);

        // Act
        var printers = service.GetPrinters();

        // Assert
        Assert.Single(printers);
        Assert.True(inner.GetPrintersWasCalled);

        var entries = provider.GetEntries();

        Assert.Contains(
            entries,
            entry =>
                entry.Level == LogLevel.Information &&
                string.Equals(entry.Message, "Printer discovery started.", StringComparison.Ordinal));

        Assert.Contains(
            entries,
            entry =>
                entry.Level == LogLevel.Information &&
                string.Equals(entry.Message, "Printer discovery completed. Found 1 printer(s).", StringComparison.Ordinal));
    }

    [Fact]
    public void GetPrinters_WhenFails_LogsErrorAndRethrows()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FailingPrinterDiscoveryService();

        var logger =
            loggerFactory.CreateLogger<LoggingPrinterService>();

        var service =
            new LoggingPrinterService(inner, logger);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => service.GetPrinters());

        var entries = provider.GetEntries();

        var error = Assert.Single(
            entries,
            entry => entry.Level == LogLevel.Error);

        Assert.Equal(
            "Printer discovery failed.",
            error.Message);

        Assert.NotNull(error.Exception);
    }

    [Fact]
    public async Task PrintAsync_WhenSuccessful_LogsStartAndCompletion()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FakePrinterService();

        var logger =
            loggerFactory.CreateLogger<LoggingPrinterService>();

        var service =
            new LoggingPrinterService(inner, logger);

        var printer = CreatePrinter();

        using var document =
            new ScannedDocument(
                new MemoryStream(new byte[] { 1, 2, 3 }));

        // Act
        await service.PrintAsync(
            printer,
            document,
            new PrintSettings());

        // Assert
        Assert.True(inner.PrintWasCalled);

        var entries = provider.GetEntries();

        Assert.Contains(
            entries,
            entry =>
                entry.Level == LogLevel.Information &&
                string.Equals(entry.Message,
                                    "Print started. Printer: Test Printer", StringComparison.Ordinal));
        Assert.Contains(
            entries,
            entry =>
                entry.Level == LogLevel.Information &&
                string.Equals(entry.Message,
                    "Print job submitted successfully. Printer: Test Printer", StringComparison.Ordinal));
    }

    [Fact]
    public async Task PrintAsync_WhenFails_LogsErrorAndRethrows()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FailingPrinterService();

        var logger =
            loggerFactory.CreateLogger<LoggingPrinterService>();

        var service =
            new LoggingPrinterService(inner, logger);

        var printer = CreatePrinter();

        using var document =
            new ScannedDocument(
                new MemoryStream(new byte[] { 1, 2, 3 }));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () =>
                service.PrintAsync(
                    printer,
                    document,
                    new PrintSettings()));

        var entries = provider.GetEntries();

        var error = Assert.Single(
            entries,
            entry => entry.Level == LogLevel.Error);

        Assert.Equal(
            "Print job failed to submit. Printer: Test Printer",
            error.Message);

        Assert.NotNull(error.Exception);
    }

    private static PrinterDevice CreatePrinter()
    {
        return new PrinterDevice
        {
            Id = "printer-1",
            Name = "Test Printer"
        };
    }

    private sealed class FakePrinterService : IPrinterService
    {
        public bool GetPrintersWasCalled { get; private set; }

        public bool PrintWasCalled { get; private set; }

        public IReadOnlyList<PrinterDevice> GetPrinters()
        {
            GetPrintersWasCalled = true;

            return new[]
            {
                CreatePrinter()
            };
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

    private sealed class FailingPrinterDiscoveryService : IPrinterService
    {
        public IReadOnlyList<PrinterDevice> GetPrinters()
        {
            throw new InvalidOperationException(
                "Test printer discovery failure.");
        }

        public Task PrintAsync(
            PrinterDevice printer,
            ScannedDocument document,
            PrintSettings settings)
        {
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