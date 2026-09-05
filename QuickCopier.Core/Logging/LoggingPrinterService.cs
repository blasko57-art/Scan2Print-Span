using Microsoft.Extensions.Logging;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.Core.Logging;

public sealed class LoggingPrinterService : IPrinterService
{
    private readonly IPrinterService _inner;
    private readonly ILogger<LoggingPrinterService> _logger;

    public LoggingPrinterService(
        IPrinterService inner,
        ILogger<LoggingPrinterService> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public IReadOnlyList<PrinterDevice> GetPrinters()
    {
        _logger.LogInformation("Printer discovery started.");

        try
        {
            var printers = _inner.GetPrinters();

            _logger.LogInformation(
                "Printer discovery completed. Found {Count} printer(s).",
                printers.Count);

            return printers;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Printer discovery failed.");

            throw;
        }
    }

    public async Task PrintAsync(
        PrinterDevice printer,
        ScannedDocument document,
        PrintSettings settings)
    {
        _logger.LogInformation(
            "Print started. Printer: {Printer}",
            printer.Name);

        try
        {
            await _inner.PrintAsync(
                printer,
                document,
                settings).ConfigureAwait(false);

            _logger.LogInformation(
                "Print job submitted successfully. Printer: {Printer}",
                printer.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Print job failed to submit. Printer: {Printer}",
                printer.Name);

            throw;
        }
    }
}
