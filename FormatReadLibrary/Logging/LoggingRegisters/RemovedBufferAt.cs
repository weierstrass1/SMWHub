using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class RemovedBufferAt : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public ILogCategory Category => new Info();
    public string MessageType => "REMOVED BUFFER AT";
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
