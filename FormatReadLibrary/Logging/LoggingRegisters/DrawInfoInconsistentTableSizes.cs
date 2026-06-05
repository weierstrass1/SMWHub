using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class DrawInfoInconsistentTableSizes : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.DRAW_INFO_INCONSISTENT_TABLE_SIZES;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public DrawInfoInconsistentTableSizes(string context)
    {
        Parameters = new Dictionary<string, string>()
        {
            { "context", context }
        };
    }
}
