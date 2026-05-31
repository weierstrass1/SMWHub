using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public class FileIsToBig : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType => "FILE IS TOO BIG";
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
