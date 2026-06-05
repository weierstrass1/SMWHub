using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class SyntaxErrorMessage : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey { get; private set; }
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public SyntaxErrorMessage(string messageType)
    {
    }
}
