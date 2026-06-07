using LogRegister;
using LogRegister.Interfaces;
using SMWHubLogging.LoggingRegisters;
using System.Collections.ObjectModel;
using Validations;

namespace SMWHubLogging;

public class LoggingEntryFactory
{
    private static readonly ReadOnlyDictionary<string, Func<ValidationError, ILoggingEntry>> _dictionary = new Dictionary<string, Func<ValidationError, ILoggingEntry>>()
    {
        { LogMessageTypeKeys.RESOURCE_NOT_FOUND, v => new ResourceNotFound(v.Context.FilePath)}
    }.AsReadOnly();
    public static ILoggingEntry CreateEntry(ValidationError error)
    {
        if (_dictionary.TryGetValue(error.MessageTypeKey, out Func<ValidationError, ILoggingEntry>? value))
            return value(error);

        ValidationContext context = error.Context;
        return new SyntaxError(context.Line, context.FilePath, context.LineContent,
            new LogEntry(error.MessageTypeKey, error.Parameters.ToDictionary()));
    }
}
