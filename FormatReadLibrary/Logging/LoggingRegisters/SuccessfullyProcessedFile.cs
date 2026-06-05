using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class SuccessfullyProcessedFile : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public string MessageTypeKey => LogMessageTypeKeys.SUCCESSFULLY_PROCESSED_FILE;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public SuccessfullyProcessedFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };

    }
}
