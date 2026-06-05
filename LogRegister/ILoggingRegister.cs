namespace LogRegister;

public interface ILoggingRegister
{
    public bool AppearWithoutVerbose { get; }
    public bool AppearInErrors { get; }
    public LogMessageType MessageType { get; }
    IReadOnlyDictionary<string, LogMessageParameter> Parameters { get; }
}
