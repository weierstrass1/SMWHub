namespace LogRegister;

public interface ILoggingEntry
{
    public bool AppearWithoutVerbose { get; }
    public bool AppearInErrors { get; }
    public string MessageTypeKey { get; }
    IReadOnlyDictionary<string, string> Parameters { get; }
}
