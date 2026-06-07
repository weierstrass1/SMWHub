using LogRegister;

namespace SMWHubLogging.LoggingRegisters;

public sealed class DynamicInfoWithoutChunks : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.DYNAMIC_INFO_WITHOUT_CHUNKS;
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public DynamicInfoWithoutChunks(string context)
    {
        Parameters = new Dictionary<string, string>
        {
            { "context", $"'{context}'" }
        }.AsReadOnly();
    }
}
