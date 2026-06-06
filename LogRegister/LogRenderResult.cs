using System.Text;

namespace LogRegister;

public delegate void LogRenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false);
public class LogRenderResult(ILogCategory category, string text, IEnumerable<LogSpan> spans)
{
    public ILogCategory Category { get; private set; } = category;
    public string Text { get; private set; } = text;
    public int Length => Text.Length;
    public IReadOnlyList<LogSpan> Spans => _spans.AsReadOnly();
    public List<LogSpan> _spans = [.. spans];
    public void RemoveOfType(SpanType type)
    {
        var spans = _spans.Where(s => s.Type == type);
        int i = 0;
        StringBuilder sb = new();
        int lastIndex = 0;
        foreach (var span in spans)
        {
            if (span.Start > lastIndex)
            {
                sb.Append(Text, lastIndex, span.Start - lastIndex);
            }
            DisplaceAll(-span.Length, i);
            lastIndex = span.Start + span.Length;
            i++;
        }
        if (lastIndex < Text.Length)
        {
            sb.Append(Text, lastIndex, Text.Length - lastIndex);
        }
        Text = sb.ToString();
        _spans = [.. _spans.Where(s => s.Type != type)];
    }
    public void DisplaceAll(int offset, int startIndex = 0)
    {
        if (startIndex >= _spans.Count)
            return;
        if (startIndex < 0)
            startIndex = 0;

        var tmpList = _spans;
        _spans = _spans[..(startIndex + 1)];
        _spans.AddRange(tmpList[(startIndex + 1)..].Select(s => s.Displace(offset)));
    }
}
