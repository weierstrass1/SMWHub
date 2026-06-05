namespace LogRegister;

public sealed class LogRenderer
{
    public void RenderAll(IEnumerable<ILoggingRegister> logRegister, LogRenderAction action, bool verbose = false, bool error = false)
    {
        foreach (var log in logRegister)
        {
            var result = Render(log);
            result.Render(action, mustWrite: (!error && (verbose || log.AppearWithoutVerbose)) || (error && log.AppearInErrors));
        }
    }
    public LogRenderResult Render(ILoggingRegister logRegister)
    {
        return logRegister.MessageType.GetMessage(logRegister);
    }
}
