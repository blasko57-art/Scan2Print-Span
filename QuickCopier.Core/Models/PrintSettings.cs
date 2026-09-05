namespace QuickCopier.Core.Models;


// default setting for printing (not hardcoded)
public class PrintSettings
{
    public string PaperSize { get; set; } = "A4";

    public int Copies { get; set; } = 1;

    public string Orientation { get; set; } = "Auto";

    public string Scaling { get; set; } = "FitToPage";
}
