using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class SuccessfullyProcessedFile : ILoggingRegister
{
    public bool AppearWithoutVerbose => false;
    public bool AppearInErrors => false;
    public ILogCategory Category => new Success();
    public string MessageType => "SUCCESSFULLY PROCESSED FILE";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public SuccessfullyProcessedFile(string filePath)
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{filePath}'" }
        };

    }
}
