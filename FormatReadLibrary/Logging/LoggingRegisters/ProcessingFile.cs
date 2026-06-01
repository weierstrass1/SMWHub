using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class ProcessingFile : ILoggingRegister
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Info();
    public string MessageType => "PROCESSING FILE";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public ProcessingFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };
    }
}
