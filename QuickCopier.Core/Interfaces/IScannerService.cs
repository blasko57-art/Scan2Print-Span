using QuickCopier.Core.Models;

namespace QuickCopier.Core.Interfaces;

public interface IScannerService
{
    Task<IReadOnlyList<ScannerDevice>> GetScannersAsync();

    Task<ScannedDocument> ScanAsync(
        ScannerDevice scanner,
        ScanSettings settings);
}
