using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class SyntaxErrorMessage : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType { get; private set; }
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public SyntaxErrorMessage(string messageType)
    {
    }
}
