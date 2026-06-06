namespace LogRegister;

public sealed class LogRenderer(LogRegisterSystem log, LogRenderAction renderAction)
{
    private readonly LogRegisterSystem _log = log;
    private readonly LogRenderAction _renderAction = renderAction;
    public void RenderAll(IEnumerable<ILoggingEntry> logEntry, bool verbose = false, bool error = false)
    {
        foreach (var log in logEntry)
        {
            Render(log, mustWrite: !error && (verbose || log.AppearWithoutVerbose));
        }
    }
    public void Render(ILoggingEntry logEntry, bool mustWrite = true)
    {
        LogMessageType messageType = _log.GetMessageType(logEntry.MessageTypeKey);
        LogRenderResult result = messageType.GetMessage(_log, logEntry);
        int cursor = 0;

        foreach (var span in result.Spans)
        {
            if (span.Start > cursor)
                _renderAction(result.Text[cursor..span.Start], result.Category, SpanType.NormalText, mustWrite: mustWrite);

            var value = result.Text.Substring(span.Start, span.Length);

            _renderAction(value, result.Category, span.Type, mustWrite: mustWrite);

            cursor = span.Start + span.Length;
        }

        if (cursor < result.Text.Length)
            _renderAction(result.Text[cursor..], result.Category, SpanType.NormalText, mustWrite: mustWrite);

        _renderAction("\n", result.Category, SpanType.NormalText, mustWrite: mustWrite);
    }
}
