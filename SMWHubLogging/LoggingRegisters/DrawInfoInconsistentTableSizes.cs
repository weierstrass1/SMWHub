using LogRegister.Interfaces;
using SMWHubValidations.FormatValidations;

namespace SMWHubLogging.LoggingRegisters;

public sealed class DrawInfoInconsistentTableSizes : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => FormatErrorsMessageTypeKeys.DRAW_INFO_INCONSISTENT_TABLE_SIZES;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public DrawInfoInconsistentTableSizes(string context)
    {
        Parameters = new Dictionary<string, string>()
        {
            { "context", context }
        };
    }
}
