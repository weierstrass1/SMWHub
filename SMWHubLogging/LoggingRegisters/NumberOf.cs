using LogRegister.Interfaces;

namespace SMWHubLogging.LoggingRegisters;

public sealed class NumberOf : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => false;
    public string MessageTypeKey => LogMessageTypeKeys.NUMBER_OF;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public NumberOf(string name, long quantity, long? size = null)
    {
        var pars = new Dictionary<string, string>
        {
            { "name", $"'{name}'" },
            { "quantity", $"{quantity}" },
            { "size", size != null ? $" ({size} bytes)" : "" }
        };

        Parameters = pars;
    }
}
