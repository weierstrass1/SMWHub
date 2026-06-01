using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class NotEnoughSpaceInROM : ILoggingRegister
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType => "NOT ENOUGH SPACE IN ROM";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public NotEnoughSpaceInROM()
    {
        Parameters = new Dictionary<string, string>().AsReadOnly();
    }
}
