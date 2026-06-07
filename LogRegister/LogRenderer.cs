using LogRegister.Interfaces;

namespace LogRegister;

public sealed class LogRenderer
{
    private readonly LogRegisterSystem _log;
    private event LogRenderAction? _renderAction;
    public LogRenderer(LogRegisterSystem log, LogRenderAction renderAction)
    {
        _log = log;
        _renderAction += renderAction;
    }
    public LogRenderer(LogRegisterSystem log, params ILogWrapper[] wrappers)
    {
        _log = log;
        foreach (var w in wrappers)
            _renderAction += w.RenderAction;
    }
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
                _renderAction?.Invoke(result.Text[cursor..span.Start], result.Category, SpanType.NormalText, mustWrite: mustWrite);

            var value = result.Text.Substring(span.Start, span.Length);

            _renderAction?.Invoke(value, result.Category, span.Type, mustWrite: mustWrite);

            cursor = span.Start + span.Length;
        }

        if (cursor < result.Text.Length)
            _renderAction?.Invoke(result.Text[cursor..], result.Category, SpanType.NormalText, mustWrite: mustWrite);

        _renderAction?.Invoke("\n", result.Category, SpanType.NormalText, mustWrite: mustWrite);
    }
}
