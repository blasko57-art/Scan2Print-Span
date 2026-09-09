using Microsoft.Extensions.Logging;

namespace QuickCopier.Core.Logging;
// logging could  be moved to App (convention) ,
//  but it is only convention and i want it here 
// (ie Core should contain   application/domain abstractions and business logic.) 


public sealed record LogEntry(
    DateTime Timestamp,
    LogLevel Level,
    string Category,
    string Message,
    Exception? Exception);
