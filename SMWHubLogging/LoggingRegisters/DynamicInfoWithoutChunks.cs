using LogRegister.Interfaces;
using SMWHubValidations.FormatValidations;

namespace SMWHubLogging.LoggingRegisters;

public sealed class DynamicInfoWithoutChunks : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => FormatErrorsMessageTypeKeys.DYNAMIC_INFO_WITHOUT_CHUNKS;
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public DynamicInfoWithoutChunks(string context)
    {
        Parameters = new Dictionary<string, string>
        {
            { "context", $"'{context}'" }
        }.AsReadOnly();
    }
}
