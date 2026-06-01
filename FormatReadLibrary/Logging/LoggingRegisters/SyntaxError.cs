using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class SyntaxError : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();

    public string MessageType => "SYNTAX ERROR";

    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public SyntaxError(int line, string file, string lineContent, string message = "")
    {
        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{file}'" },
            { "line", $"'{line+1}'" },
            { "message", string.IsNullOrWhiteSpace(message) ?
                "" :
                $".\n\t\t{message}"},
            { "lineContent", $"'{lineContent}'"   }
        }.AsReadOnly();
    }
}
