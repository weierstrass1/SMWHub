namespace LogRegister;

public delegate void LogRenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false);
public struct LogRenderResult
{
    public ILogCategory Category;
    public string Text;
    public IReadOnlyList<LogSpan> Spans;
}
