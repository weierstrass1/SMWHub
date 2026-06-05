namespace LogRegister;

public class LogEntry : ILoggingEntry
{
    public bool AppearWithoutVerbose { get; set; }
    public bool AppearInErrors { get; set; }
    public string MessageTypeKey { get; private set; }
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public LogEntry(string messageTypeKey)
    {
        MessageTypeKey = messageTypeKey;
        Parameters = new Dictionary<string, string>().AsReadOnly();
    }
    public LogEntry(string messageTypeKey, Dictionary<string, string> parameters)
    {
        MessageTypeKey = messageTypeKey;
        Parameters = parameters != null ?
            parameters.AsReadOnly() :
            new Dictionary<string, string>().AsReadOnly();
    }
}
