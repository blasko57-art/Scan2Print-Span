using QuickCopier.Core.Models;

namespace QuickCopier.Core.Interfaces;

public interface ICopyService
{
    Task CopyAsync(
        ScannerDevice scanner,
        PrinterDevice printer,
        ScanSettings scanSettings,
        PrintSettings printSettings);
}
