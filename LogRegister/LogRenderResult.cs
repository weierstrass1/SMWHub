namespace LogRegister;

public delegate void LogRenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false);
public class LogRenderResult(ILogCategory category, string text, IEnumerable<LogSpan> spans)
{
    public ILogCategory Category { get; private set; } = category;
    public string Text { get; private set; } = text;
    public IReadOnlyList<LogSpan> Spans => _spans.AsReadOnly();
    public List<LogSpan> _spans = [.. spans];
    public void RemoveOfType(SpanType type)
    {
        var spans = _spans.Where(s => s.Type == type);
        int i = 0;
        foreach(var span in spans)
        {
            i++;
            DisplaceAll(-span.Length, i);
        }
        _spans = [.. _spans.Where(s => s.Type != type)];
    }
    public void DisplaceAll(int offset, int startIndex = 0)
    {
        if (startIndex >= _spans.Count)
            return;
        if (startIndex < 0)
            startIndex = 0;

        var tmpList = _spans;
        _spans = _spans[..startIndex];
        _spans.AddRange(tmpList[startIndex..].Select(s => s.Displace(offset)));
    }
}
