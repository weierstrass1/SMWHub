using LogRegister.Interfaces;
using SMWHubValidations.FormatValidations;

namespace SMWHubLogging.LoggingRegisters;

public sealed class FailedToProcessFile : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => FormatErrorsMessageTypeKeys.FAILED_TO_PROCESS_FILE;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public FailedToProcessFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };

    }
}
