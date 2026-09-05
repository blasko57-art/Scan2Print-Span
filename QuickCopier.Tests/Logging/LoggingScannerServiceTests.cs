using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Logging;
using QuickCopier.Core.Models;

namespace QuickCopier.Tests.Logging;

public class LoggingScannerServiceTests
{
    [Fact]
    public async Task ScanAsync_WhenSuccessful_LogsStartAndCompletion()
    {
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FakeScannerService();

        var logger =
            loggerFactory.CreateLogger<LoggingScannerService>();

        var service =
            new LoggingScannerService(inner, logger);

        var scanner = new ScannerDevice
        {
            Id = "scanner-1",
            Name = "Test Scanner"
        };

        //await using var _ = Task.CompletedTask as IAsyncDisposable;
        //unnecessary

        using var document =
            await service.ScanAsync(
                scanner,
                new ScanSettings());

        var entries = provider.GetEntries();

        Assert.Contains(
            entries,
            e => e.Level == LogLevel.Information &&
                 e.Message.Contains("Scan started"));

        Assert.Contains(
            entries,
            e => e.Level == LogLevel.Information &&
                 e.Message.Contains("Scan completed successfully"));
    }

    [Fact]
    public async Task ScanAsync_WhenFails_LogsErrorAndRethrows()
    {
        using var provider = new InMemoryLoggerProvider();

        using var loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(provider);
            });

        var inner = new FailingScannerService();

        var logger =
            loggerFactory.CreateLogger<LoggingScannerService>();

        var service =
            new LoggingScannerService(inner, logger);

        var scanner = new ScannerDevice
        {
            Id = "scanner-1",
            Name = "Test Scanner"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ScanAsync(
                scanner,
                new ScanSettings()));

        var entries = provider.GetEntries();

        var error = Assert.Single(
            entries,
            e => e.Level == LogLevel.Error);



        Assert.Contains("Scan failed", error.Message);
        Assert.NotNull(error.Exception);
    }

    private sealed class FakeScannerService : IScannerService
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
            return Task.FromResult(
                new ScannedDocument(
                    new MemoryStream(new byte[] { 1, 2, 3 })));
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
}
