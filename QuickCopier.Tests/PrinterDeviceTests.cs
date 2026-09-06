using QuickCopier.Core.Models;

namespace QuickCopier.Tests;

public class PrinterDeviceTests
{
    [Fact]
    public void NewPrinterDevice_HasEmptyIdAndName()
    {
        // Arrange
        var printer = new PrinterDevice();

        // Assert
        Assert.Equal(string.Empty, printer.Id);
        Assert.Equal(string.Empty, printer.Name);
    }

    [Fact]
    public void DefaultValues_AreEmptyStrings()
    {
        // Arrange
        var printer = new PrinterDevice();

        // Assert
        Assert.Equal(string.Empty, printer.Id);
        Assert.Equal(string.Empty, printer.Name);
    }

    [Fact]
    public void IdAndName_CanBeAssigned()
    {
        // Arrange
        var printer = new PrinterDevice
        {
            Id = "printer-123",
            Name = "Test Printer"
        };

        // Assert
        Assert.Equal("printer-123", printer.Id);
        Assert.Equal("Test Printer", printer.Name);
    }
}
