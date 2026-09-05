namespace QuickCopier.Core.Models;

// default settings (not hardcoded)
public class ScanSettings
{
    public int Dpi { get; set; } = 300;

    public string ColorMode { get; set; } = "Color";

    public string PaperSize { get; set; } = "A4";

    public string Source { get; set; } = "Auto";
    public bool UseFeeder { get; set; } = false;

}
