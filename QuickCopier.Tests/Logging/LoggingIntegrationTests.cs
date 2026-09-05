using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickCopier.Core.Logging;

namespace QuickCopier.Tests.Logging;

public class LoggingIntegrationTests
{
    [Fact]
    public void DependencyInjection_CreatesLogger()
    {
        // Arrange
        var provider = new InMemoryLoggerProvider();

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(provider);
        });

        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var logger =
            serviceProvider.GetRequiredService<ILogger<LoggingIntegrationTests>>();

        logger.LogInformation("Test message.");

        // Assert
        var entry = Assert.Single(provider.Entries);

        Assert.Equal(
            LogLevel.Information,
            entry.Level);

        Assert.Equal(
            "Test message.",
            entry.Message);
    }
}
