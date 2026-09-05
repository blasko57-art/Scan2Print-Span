using QuickCopier.Core.Models;

namespace QuickCopier.Tests;

public class ScannedDocumentTests
{
    [Fact]
    public void Constructor_StoresProvidedStream()
    {
        using var stream = new MemoryStream();

        using var document = new ScannedDocument(stream);

        Assert.Same(stream, document.Data);
    }

    [Fact]
    public void Constructor_ThrowsWhenDataIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ScannedDocument(null!));
    }

    [Fact]
    public void Dispose_DisposesOwnedStream()
    {
        var stream = new MemoryStream();
        var document = new ScannedDocument(stream);

        document.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => _ = stream.Length);
    }

    [Fact]
    public void Metadata_CanBeAssigned()
    {
        using var stream = new MemoryStream();

        using var document = new ScannedDocument(stream)
        {
            Width = 2480,
            Height = 3508,
            Dpi = 300
        };

        Assert.Equal(2480, document.Width);
        Assert.Equal(3508, document.Height);
        Assert.Equal(300, document.Dpi);
    }
}
