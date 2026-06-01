using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class FailedToProcessFile : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType => "FAILED TO PROCESS FILE";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public FailedToProcessFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };

    }
}
