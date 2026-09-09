using QuickCopier.Core.Models;

namespace QuickCopier.App.State;

public sealed class DocumentState : IDisposable
{
    private ScannedDocument? _document;

    public ScannedDocument? Document =>
        _document;

    public bool HasDocument =>
        _document is not null;

    public void SetDocument(ScannedDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        Release();

        _document = document;
    }


    public ScannedDocument? TakeDocument()
    {
        var document = _document;

        _document = null;

        return document;
    }

    public void Release()
    {
        _document?.Dispose();

        _document = null;
    }

    public void Dispose()
    {
        Release();
    }
}