using QuickCopier.App.State;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.App.Workflows;
// default settings for now 
// later add settings like in normal scannoign and printing
public sealed class DocumentWorkflow
{
private readonly IScannerService _scannerService;
private readonly IPrinterService _printerService;
private readonly ICopyService _copyService;
private readonly DocumentState _documentState;

    public DocumentWorkflow(
        IScannerService scannerService,
        IPrinterService printerService,
        ICopyService copyService,
        DocumentState documentState)
    {
        _scannerService = scannerService;
        _printerService = printerService;
        _copyService = copyService;
        _documentState = documentState;
    }

    public bool HasDocument =>
        _documentState.HasDocument;

    public async Task<ScannedDocument> ScanAsync(
        ScannerDevice scanner)
    {
        ArgumentNullException.ThrowIfNull(scanner);

        var document =
            await _scannerService.ScanAsync(
                scanner,
                new ScanSettings());

        _documentState.SetDocument(document);

        return document;
    }

    public async Task PrintAsync(
        PrinterDevice printer)
    {
        var document =
            _documentState.Document;

        if (document is null)
        {
            throw new InvalidOperationException(
                "There is no scanned document in RAM.");
        }

        await _printerService.PrintAsync(
            printer,
            document,
            new PrintSettings());

        _documentState.Release();
    }

    public async Task CopyAsync(
        ScannerDevice scanner,
        PrinterDevice printer)
    {
        await _copyService.CopyAsync(
            scanner,
            printer,
            new ScanSettings(),
            new PrintSettings());
    }

    public void ReleaseDocument()
    {
        _documentState.Release();
    }

}