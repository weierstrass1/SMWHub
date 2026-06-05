using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class FileIsToBig : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.FILE_IS_TOO_BIG;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public FileIsToBig(string filePath, long fileSize, long maxSize)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" },
            { "size", $"{fileSize} bytes" },
            { "maxSize", $"{maxSize} bytes" }
        };
    }
}
