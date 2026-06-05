using LogRegister;

namespace FormatReadLibrary.Logging.LoggingRegisters;

public sealed class Title : ILoggingEntry
{
    public bool AppearWithoutVerbose { get; set; } = false;
    public bool AppearInErrors { get; set; } = false;
    public string MessageTypeKey => LogMessageTypeKeys.TITLE;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public Title(string title)
    {
        Parameters = new Dictionary<string, string>
        {
            { "title", $"{title}" }
        };
    }
}
