using System.Text;
using NAPS2.Wia;

namespace QuickCopier.Windows.Scanning;

public sealed class WindowsScannerCapabilities
{
    public string Inspect(string scannerId) //TODO change to static maybe 
    {
        // null check
        // ID format is determined by WIA/NAPS2
        // so  ID could THEORETICALLY be with whitespace, so do not use ThrowIfNullOrWhiteSpace 
        if (string.IsNullOrEmpty(scannerId))
        {
            throw new ArgumentException(
                "Scanner ID cannot be null or empty.",
                nameof(scannerId));
        }
        var output = new StringBuilder();

        using var deviceManager = new WiaDeviceManager();

        using var device = deviceManager.FindDevice(scannerId);
        var items = device.GetSubItems();
        foreach (var item in items)
        {
            try
            {
                output.AppendLine();
                output.AppendLine($"ITEM: {item.Name()}");
                output.AppendLine("----------------------------------------");

                foreach (var property in item.Properties)
                {
                    output.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"ID={property.Id}, " +
                        $"Name=\"{property.Name}\", " +
                        $"Type={property.Type}");

                    try
                    { output.AppendLine($"  Value: {property.Value}"); }
                    catch (Exception ex)
                    { output.AppendLine($"  Value: <unavailable: {ex.Message}>"); }

                    try
                    {
                        var attributes = property.Attributes;

                        output.AppendLine(
                            $"  Flags: {attributes.Flags}");

                        output.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"  Range: " +
                            $"Min={attributes.Min}, " +
                            $"Nom={attributes.Nom}, " +
                            $"Max={attributes.Max}, " +
                            $"Step={attributes.Step}");

                        if (attributes.Values != null)
                        {
                            output.AppendLine($"  Values: " + $"{string.Join(", ", attributes.Values)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        output.AppendLine(
                            $"  Attributes: " +
                            $"<unavailable: {ex.Message}>");
                    }
                    output.AppendLine();
                }
            }
            finally
            { item.Dispose(); }
        }

        return output.ToString();
    }
}