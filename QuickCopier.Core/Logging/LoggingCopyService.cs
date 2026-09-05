// logging decorator 
using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.Core.Logging;

public sealed class LoggingCopyService : ICopyService
{
    private readonly ICopyService _inner;
    private readonly ILogger<LoggingCopyService> _logger;

    public LoggingCopyService(
        ICopyService inner,
        ILogger<LoggingCopyService> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task CopyAsync(
        ScannerDevice scanner,
        PrinterDevice printer,
        ScanSettings scanSettings,
        PrintSettings printSettings)
    {
        _logger.LogInformation(
            "Copy started. Scanner: {Scanner}, Printer: {Printer}",
            scanner.Name,
            printer.Name);

        try
        {
            await _inner.CopyAsync(
                scanner,
                printer,
                scanSettings,
                printSettings).ConfigureAwait(false);

            _logger.LogInformation(
                   "Copy completed successfully. Scanner: {Scanner}, Printer: {Printer}",
                scanner.Name,
                printer.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Copy failed. Scanner: {Scanner}, Printer: {Printer}",
                scanner.Name,
                printer.Name);

            throw;
        }
    }
}
