using Microsoft.Extensions.Logging;

namespace QuickCopier.Core.Logging;


internal sealed class InMemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly List<LogEntry> _entries;

    public InMemoryLogger(
        string categoryName,
        List<LogEntry> entries)
    {
        _categoryName = categoryName;
        _entries = entries;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        lock (_entries)
        {
            _entries.Add(
                new LogEntry(
                    DateTime.UtcNow,
                    logLevel,
                    _categoryName,
                    message,
                    exception));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
