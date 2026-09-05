using Microsoft.Extensions.Logging;
using QuickCopier.Core.Logging;

// tests for logging part
namespace QuickCopier.Tests;

public class InMemoryLoggerTests
{
    [Fact]
    public void Log_AddsEntryToProvider()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        var logger = provider.CreateLogger("TestCategory");

        // Act
        logger.LogInformation("Hello from test.");

        // Assert
        var entry = Assert.Single(provider.Entries);

        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal("TestCategory", entry.Category);
        Assert.Equal("Hello from test.", entry.Message);
        Assert.Null(entry.Exception);
    }
    [Fact]
    public void Clear_RemovesAllEntries()
    {
        using var provider = new InMemoryLoggerProvider();

        var logger = provider.CreateLogger("TestCategory");

        logger.LogInformation("Test message.");

        Assert.Single(provider.Entries);

        provider.Clear();

        Assert.Empty(provider.Entries);
    }
    [Fact]
    public void Log_WithException_StoresException()
    {
        // Arrange
        using var provider = new InMemoryLoggerProvider();

        var logger = provider.CreateLogger("TestCategory");

        var exception = new InvalidOperationException(
            "Test exception.");

        // Act
        logger.LogError(
            exception,
            "Something went wrong.");

        // Assert
        var entry = Assert.Single(provider.Entries);

        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Equal("Something went wrong.", entry.Message);
        Assert.Same(exception, entry.Exception);
    }
}