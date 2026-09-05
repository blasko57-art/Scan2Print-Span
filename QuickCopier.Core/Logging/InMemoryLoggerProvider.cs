using Microsoft.Extensions.Logging;

namespace QuickCopier.Core.Logging;


public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly List<LogEntry> _entries = new();

    public IReadOnlyList<LogEntry> GetEntries()
    {
        lock (_entries)
        {
            return _entries.ToList();
        }
    }
    public IReadOnlyList<LogEntry> Entries => GetEntries();

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(categoryName, _entries);
    }
    public void Clear()
    {
        lock (_entries)
        {
            _entries.Clear();
        }
    }

    public void Dispose()
    {
    }
}
