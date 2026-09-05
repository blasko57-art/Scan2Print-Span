namespace QuickCopier.Core.Models;

// settings for app
public class AppSettings
{
    public string? DefaultScannerId { get; set; }

    public string? DefaultPrinterId { get; set; }

    public ScanSettings Scan { get; set; } = new();

    public PrintSettings Print { get; set; } = new();
}
