using System.Drawing.Printing;
using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.Windows.Printing;

public class WindowsPrinterService : IPrinterService
{
    public IReadOnlyList<PrinterDevice> GetPrinters()
    {
        var printers = new List<PrinterDevice>();

        foreach (string printerName in PrinterSettings.InstalledPrinters)
        {
            printers.Add(new PrinterDevice
            {
                Id = printerName,
                Name = printerName
            });
        }

        return printers;
    }

    public Task PrintAsync(
        PrinterDevice printer,
        ScannedDocument document,
        PrintSettings settings)
    {

        ArgumentNullException.ThrowIfNull(printer);
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(settings);


        if (document.Data.Length == 0)
        {
            throw new InvalidOperationException(
                "The scanned document contains no data.");
        }

        document.Data.Position = 0;
// The scanned image is kept in RAM only; no temporary file is created.
// do not write anything that would save it somewhere !
        using var image =
            Image.FromStream(document.Data);

        using var printDocument =
            new PrintDocument();

        printDocument.PrinterSettings.PrinterName =
            printer.Name;

        if (!printDocument.PrinterSettings.IsValid)
        {
            throw new InvalidOperationException(
                $"Printer '{printer.Name}' is not available.");
        }

        printDocument.DocumentName =
            "QuickCopier";

        printDocument.PrinterSettings.Copies =
            (short)Math.Max(1, settings.Copies);

        if (settings.Orientation.Equals(
                "Landscape",
                StringComparison.OrdinalIgnoreCase))
        {
            printDocument.DefaultPageSettings.Landscape = true;
        }
        else if (settings.Orientation.Equals(
                     "Portrait",
                     StringComparison.OrdinalIgnoreCase))
        {
            printDocument.DefaultPageSettings.Landscape = false;
        }

        printDocument.PrintPage += (_, e) =>
        {
            var printableArea = e.MarginBounds;
            // Fit the scanned image inside the printable area
            // while preserving its original aspect ratio.
            float scaleX =
                (float)printableArea.Width / image.Width;

            float scaleY =
                (float)printableArea.Height / image.Height;

            float scale =
                Math.Min(scaleX, scaleY);

            int width =
                (int)(image.Width * scale);

            int height =
                (int)(image.Height * scale);

            int x =
                printableArea.Left +
                (printableArea.Width - width) / 2;

            int y =
                printableArea.Top +
                (printableArea.Height - height) / 2;


            if (e.Graphics is null)
            {
                throw new InvalidOperationException(
                    "The printer graphics context was unavailable.");
            }

            e.Graphics.DrawImage(
                image,
                new Rectangle(
                    x,
                    y,
                    width,
                    height));

            e.HasMorePages = false;
        };

        printDocument.Print();

        return Task.CompletedTask;
    }


}