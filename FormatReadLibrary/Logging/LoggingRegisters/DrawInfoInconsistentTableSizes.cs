using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class DrawInfoInconsistentTableSizes : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType => "DRAW INFO INCONSISTENT TABLE SIZES";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public DrawInfoInconsistentTableSizes(string context)
    {
        Parameters = new Dictionary<string, string>()
        {
            { "context", context }
        };
    }
}
