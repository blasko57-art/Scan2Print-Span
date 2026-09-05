using QuickCopier.Core.Models;

namespace QuickCopier.Tests;

public class PrintSettingsTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // Arrange & Act
        var settings = new PrintSettings();

        // Assert
        Assert.Equal("A4", settings.PaperSize);
        Assert.Equal(1, settings.Copies);
        Assert.Equal("Auto", settings.Orientation);
        Assert.Equal("FitToPage", settings.Scaling);
    }

    [Fact]
    public void Properties_CanBeChanged()
    {
        // Arrange
        var settings = new PrintSettings();

        // Act
        settings.PaperSize = "Letter";
        settings.Copies = 3;
        settings.Orientation = "Landscape";
        settings.Scaling = "ActualSize";

        // Assert
        Assert.Equal("Letter", settings.PaperSize);
        Assert.Equal(3, settings.Copies);
        Assert.Equal("Landscape", settings.Orientation);
        Assert.Equal("ActualSize", settings.Scaling);
    }
}
