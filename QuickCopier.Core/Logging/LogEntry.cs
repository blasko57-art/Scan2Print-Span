using Microsoft.Extensions.Logging;

namespace QuickCopier.Core.Logging;
//Core should contain   application/domain abstractions and business logic.
// logging should be moved to App probably, but for now it can stay here...


public sealed record LogEntry(
    DateTime Timestamp,
    LogLevel Level,
    string Category,
    string Message,
    Exception? Exception);
