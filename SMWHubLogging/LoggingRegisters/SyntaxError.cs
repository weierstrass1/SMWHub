using LogRegister;

namespace SMWHubLogging.LoggingRegisters;

public sealed class SyntaxError : ILoggingEntryWithNestedMessage
{
    public bool AppearWithoutVerbose => true;
    public bool AppearInErrors => true;
    public string MessageTypeKey => LogMessageTypeKeys.SYNTAX_ERROR;
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public IReadOnlyDictionary<string, ILoggingEntry> NestedEntries { get; private set; }
    private static string[] _parameterNames = [
            "file",
            "line",
            "lineContent"
        ];
    public SyntaxError(int line, string file, string lineContent, ILoggingEntry entry)
    {
        NestedEntries = new Dictionary<string, ILoggingEntry>()
        {
            { "message", entry }
        }.AsReadOnly();

        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{file}'" },
            { "line", $"'{line+1}'" },
            { "lineContent", $"'{lineContent}'"   }
        }.AsReadOnly();
    }
    public SyntaxError(int line, string file, string lineContent, string message = "")
    {
        NestedEntries = new Dictionary<string, ILoggingEntry>()
        {
            { "message", new LogEntry(LogMessageTypeKeys.RAW_SYNTAX_ERROR_MESSAGE, new Dictionary<string, string>(){
                {"message",message}
            }) }
        }.AsReadOnly();

        Parameters = new Dictionary<string, string>
        {
            { "file", $"'{file}'" },
            { "line", $"'{line+1}'" },
            { "lineContent", $"'{lineContent}'"   }
        }.AsReadOnly();
    }
    public SyntaxError(string nestedMessageType, IDictionary<string, string> parameters)
    {
        Parameters = parameters
            .Where(p => _parameterNames.Contains(p.Key))
            .ToDictionary(p => p.Key, p => $"'{p.Value}'");
    }
}
