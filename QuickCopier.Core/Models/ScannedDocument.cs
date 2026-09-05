namespace QuickCopier.Core.Models;
// this code  is very important (because of RAM-only requirement.)

//IDisposable to explicitly relase memory when not needed anymore
// Memory stream data is place where scanned document is located (no pdf, png, jpg)
// no file is created ordisk (no temporary, no permanent)
public sealed class ScannedDocument : IDisposable
{
    public MemoryStream Data { get; }

    public int Width { get; init; }

    public int Height { get; init; }

    public int Dpi { get; init; }

    public ScannedDocument(MemoryStream data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public void Dispose()
    {
        Data.Dispose();
        // even if printing fails, scanned document in RAM gets cleaned up
    }
}
