using LogRegister;
using FormatReadLibrary.Logging.Categories;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class DynamicInfoSizeMismatch : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public ILogCategory Category => new Error();
    public string MessageType => "DYNAMIC INFO SIZE MISMATCH";
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public DynamicInfoSizeMismatch(string contextName, long size1, long size2)
    {
        Parameters = new Dictionary<string, string>
        {
            { "context", $"'{contextName}'" },
            { "size1", $"{size1} bytes" },
            { "size2", $"{size2} bytes" }
        };
    }
}
