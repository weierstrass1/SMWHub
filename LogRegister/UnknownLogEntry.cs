using LogRegister.Interfaces;

namespace LogRegister;

public class UnknownLogEntry : ILoggingEntry
{
    public bool AppearWithoutVerbose => false;

    public bool AppearInErrors => true;

    public string MessageTypeKey => UnknownCategory.KEY;

    public IReadOnlyDictionary<string, string> Parameters { get; }
    public UnknownLogEntry(string key)
    {
        Parameters = new Dictionary<string, string>()
        {
            { "key", $"'{key}'"}
        }.AsReadOnly();
    }
}
