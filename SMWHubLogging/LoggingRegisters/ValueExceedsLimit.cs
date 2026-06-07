using LogRegister.Interfaces;

namespace SMWHubLogging.LoggingRegisters;
public sealed class ValueExceedsLimit : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.VALUE_EXCEEDS_LIMIT;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public ValueExceedsLimit(string context, string parameter, int value, int limit)
    {
        Parameters = new Dictionary<string, string>()
        {
            { "context", context },
            { "parameter", parameter },
            { "value", $"${value:X4}" },
            { "limit", $"${limit:X2}" }
        };
    }
}
