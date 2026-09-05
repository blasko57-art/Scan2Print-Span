using QuickCopier.Core.Models;

namespace QuickCopier.Core.Interfaces;

public interface IPrinterService
{
    IReadOnlyList<PrinterDevice> GetPrinters();

    Task PrintAsync(
        PrinterDevice printer,
        ScannedDocument document,
        PrintSettings settings);
}
