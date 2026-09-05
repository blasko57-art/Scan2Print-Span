using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.Core.Logging;

public sealed class LoggingScannerService : IScannerService
{
    private readonly IScannerService _inner;
    private readonly ILogger<LoggingScannerService> _logger;


    public LoggingScannerService(
        IScannerService inner,
        ILogger<LoggingScannerService> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ScannerDevice>> GetScannersAsync()
    {
        _logger.LogInformation("Scanner discovery started.");

        try
        {
            var scanners = await _inner.GetScannersAsync();

            _logger.LogInformation(
                "Scanner discovery completed. Found {Count} scanner(s).",
                scanners.Count);

            return scanners;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Scanner discovery failed.");

            throw;
        }
    }

    public async Task<ScannedDocument> ScanAsync(
        ScannerDevice scanner,
        ScanSettings settings)
    {
        // if someone called await service.ScanAsync(null!, settings);
    ArgumentNullException.ThrowIfNull(scanner);
    ArgumentNullException.ThrowIfNull(settings);
        _logger.LogInformation(
            "Scan started. Scanner: {Scanner}",
            scanner.Name);

        try
        {
            var document =
                await _inner.ScanAsync(
                    scanner,
                    settings);

            _logger.LogInformation(
                "Scan completed successfully. Scanner: {Scanner}",
                scanner.Name);

            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Scan failed. Scanner: {Scanner}",
                scanner.Name);

            throw;
        }
    }
}
