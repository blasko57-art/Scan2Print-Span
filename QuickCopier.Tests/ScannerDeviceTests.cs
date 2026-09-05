using QuickCopier.Core.Models;

namespace QuickCopier.Tests;

public class ScannerDeviceTests
{
    [Fact]
    public void ToString_ReturnsScannerName()
    {
        // Arrange
        var scanner = new ScannerDevice
        {
            Id = "scanner-1",
            Name = "My Scanner"
        };

        // Act
        var result = scanner.ToString();

        // Assert
        Assert.Equal("My Scanner", result);
    }

    [Fact]
    public void IdAndName_CanBeAssigned()
    {
        // Arrange
        var scanner = new ScannerDevice
        {
            Id = "scanner-123",
            Name = "Test Scanner"
        };

        // Assert
        Assert.Equal("scanner-123", scanner.Id);
        Assert.Equal("Test Scanner", scanner.Name);
    }
}
