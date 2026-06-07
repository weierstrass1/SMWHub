using LogRegister.Interfaces;

namespace SMWHubLogging.LoggingRegisters;

public sealed class RemovedBufferAt : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public string MessageTypeKey => LogMessageTypeKeys.REMOVED_BUFFER_AT;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public RemovedBufferAt(long address, long size)
    {
        Parameters = new Dictionary<string, string>
        {
            { "address", $"'0x{address:X6}'" },
            { "size", $"{size} bytes" }
        };
    }
}
