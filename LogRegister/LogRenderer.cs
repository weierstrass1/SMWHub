namespace LogRegister;

public sealed class LogRenderer(LogRegisterSystem log)
{
    private readonly LogRegisterSystem _log = log;
    public void RenderAll(IEnumerable<ILoggingEntry> logEntry, LogRenderAction action, bool verbose = false, bool error = false)
    {
        foreach (var log in logEntry)
        {
            var result = Render(log);
            result.Render(action, mustWrite: (!error && (verbose || log.AppearWithoutVerbose)) || (error && log.AppearInErrors));
        }
    }
    public LogRenderResult Render(ILoggingEntry logEntry)
    {
        LogMessageType messageType = _log.GetMessageType(logEntry.MessageTypeKey);
        return messageType.GetMessage(logEntry);
    }
}
