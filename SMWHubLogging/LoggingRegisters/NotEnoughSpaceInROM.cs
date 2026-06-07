using LogRegister;

namespace SMWHubLogging.LoggingRegisters;

public sealed class NotEnoughSpaceInROM : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.NOT_ENOUGH_SPACE_IN_ROM;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public NotEnoughSpaceInROM()
    {
        Parameters = new Dictionary<string, string>().AsReadOnly();
    }
}
