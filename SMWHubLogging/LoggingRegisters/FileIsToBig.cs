using LogRegister.Interfaces;
using SMWHubValidations.FormatValidations;

namespace SMWHubLogging.LoggingRegisters;

public sealed class FileIsToBig : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => FormatErrorsMessageTypeKeys.FILE_IS_TOO_BIG;
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
