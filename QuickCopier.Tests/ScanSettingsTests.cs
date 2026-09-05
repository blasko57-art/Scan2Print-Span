using QuickCopier.Core.Models;

namespace QuickCopier.Tests;

public class ScanSettingsTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // Arrange & Act
        var settings = new ScanSettings();

        // Assert
        Assert.Equal(300, settings.Dpi);
        Assert.Equal("Color", settings.ColorMode);
        Assert.Equal("A4", settings.PaperSize);
        Assert.Equal("Auto", settings.Source);
        Assert.False(settings.UseFeeder);
    }

    [Fact]
    public void Properties_CanBeChanged()
    {
        // Arrange
        var settings = new ScanSettings();

        // Act
        settings.Dpi = 600;
        settings.ColorMode = "Grayscale";
        settings.PaperSize = "Letter";
        settings.Source = "Feeder";
        settings.UseFeeder = true;

        // Assert
        Assert.Equal(600, settings.Dpi);
        Assert.Equal("Grayscale", settings.ColorMode);
        Assert.Equal("Letter", settings.PaperSize);
        Assert.Equal("Feeder", settings.Source);
        Assert.True(settings.UseFeeder);
    }
}
