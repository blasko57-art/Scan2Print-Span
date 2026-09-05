//using Microsoft.Extensions.Logging;
// logging will be moved to different code in different folder 
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.Core.Services;

public sealed class CopyService : ICopyService
{
    private readonly IScannerService _scannerService;
    private readonly IPrinterService _printerService;

    public CopyService(
        IScannerService scannerService,
        IPrinterService printerService)
    {
        _scannerService = scannerService;
        _printerService = printerService;
    }

    public async Task CopyAsync(
        ScannerDevice scanner,
        PrinterDevice printer,
        ScanSettings scanSettings,
        PrintSettings printSettings)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        ArgumentNullException.ThrowIfNull(printer);
        ArgumentNullException.ThrowIfNull(scanSettings);
        ArgumentNullException.ThrowIfNull(printSettings);

        ScannedDocument? document = null;

        try
        {
            document = await _scannerService.ScanAsync(
                scanner,
                scanSettings).ConfigureAwait(false);

            await _printerService.PrintAsync(
                printer,
                document,
                printSettings).ConfigureAwait(false);
        }
        finally
        {
            document?.Dispose();
        }
    }
}
