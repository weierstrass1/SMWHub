using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class ProcessingFile : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.PROCESSING_FILE;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public ProcessingFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };
    }
}
