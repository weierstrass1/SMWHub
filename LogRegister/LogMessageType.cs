using System.Text;
using System.Text.RegularExpressions;

namespace LogRegister;

public sealed partial class LogMessageType
{
    public string Key { get; private set; }
    public string Text { get; private set; }
    public ILogCategory Category { get; private init; }
    private LogSpan _prefixSpan;
    private readonly Dictionary<string, LogSpan> _variables;
    public LogMessageType(string key, string text, ILogCategory category)
    {
        Key = key;
        Text = $"[{Key}]: {text}";
        Category = category;
        _prefixSpan = new()
        {
            Category = Category,
            Start = 0,
            Length = Key.Length + 4,
            Type = SpanType.Prefix
        };
        _variables = [];
        foreach (Match match in ParameterRegex().Matches(Text))
        {
            _variables.Add(match.Value.Replace("{", "").Replace("}", ""), new()
            {
                Category = Category,
                Start = match.Index,
                Length = match.Length,
                Type = match.Groups["nested"].Success ?
                    SpanType.NestedMessage :
                    SpanType.Parameter
            });
        }
    }
    public static LogMessageType FromDTO(LogRegisterSystem log, string category, LogMessageTypeDTO messageDTO)
    {
        ILogCategory? cat = log.GetCategory(category) ?? 
            throw new InvalidOperationException($"Error in Message \"{category}\": Category {messageDTO.Category} doesn't exist.");
        LogMessageType reg = new(category, messageDTO.Message, cat);

        return reg;
    }
    public void Validate(ILoggingEntry entry)
    {
        string? param = entry.Parameters.Keys.FirstOrDefault(p => !_variables.ContainsKey(p));
        if (param != null)
            throw new InvalidOperationException($"Parameter {param} is invalid.");
        string? variable = _variables.Keys.FirstOrDefault(v => !entry.Parameters.ContainsKey(v));
        if(variable != null)
            throw new InvalidOperationException($"Parameter {variable} is missing.");
    }
    public LogRenderResult GetMessage(ILoggingEntry logEntry)
    {
        Dictionary<string, LogRenderResult> nestedMessages = getNestedResults(logEntry);
        var sb = new StringBuilder();
        int lastIndex = 0;

        List<LogSpan> spans = [_prefixSpan];
        LogRenderResult? nestedResult;
        int offset = 0;

        foreach ((string key, LogSpan span) in _variables)
        {
            appendTextBeforeSpan(sb, lastIndex, span);

            if (span.Type == SpanType.NestedMessage && nestedMessages.TryGetValue(key, out nestedResult))
                offset = appendNestedResult(sb, spans, nestedResult, offset);
            else
                lastIndex = appendRegularSpan(logEntry, sb, spans, offset, key, span);
        }
        appendTextAfterLastSpan(sb, lastIndex);
        return new(Category, sb.ToString(), spans.AsReadOnly());
    }
    private Dictionary<string, LogRenderResult> getNestedResults(ILoggingEntry logEntry)
    {
        Dictionary<string, LogRenderResult> nestedMessages = [];
        if (logEntry is ILoggingEntryWithNestedMessage loggingEntryWithNestedMessage)
        {
            nestedMessages = loggingEntryWithNestedMessage
                .NestedEntries
                .ToDictionary(ne => ne.Key, ne => GetMessage(ne.Value));
        }

        return nestedMessages;
    }
    private void appendTextBeforeSpan(StringBuilder sb, int lastIndex, LogSpan span)
    {
        if (span.Start > lastIndex)
        {
            sb.Append(Text, lastIndex, span.Start - lastIndex);
        }
    }
    private static int appendNestedResult(StringBuilder sb, List<LogSpan> spans, LogRenderResult nestedResult, int offset)
    {
        nestedResult.RemoveOfType(SpanType.Prefix);
        nestedResult.DisplaceAll(offset);
        foreach (var s in nestedResult.Spans)
            spans.Add(s);
        sb.Append(nestedResult.Text);
        offset += nestedResult.Text.Length;
        return offset;
    }
    private int appendRegularSpan(ILoggingEntry logEntry, StringBuilder sb, List<LogSpan> spans, int offset, string key, LogSpan span)
    {
        int lastIndex;
        if (!logEntry.Parameters.TryGetValue(key, out string? value))
            value = $"{{{key}}}";

        int start = sb.Length;

        sb.Append(value);

        spans.Add(new()
        {
            Category = Category,
            Start = start + offset,
            Length = value.Length,
            Type = SpanType.Parameter
        });

        lastIndex = span.Start + span.Length;
        return lastIndex;
    }
    private void appendTextAfterLastSpan(StringBuilder sb, int lastIndex)
    {
        if (lastIndex < Text.Length)
        {
            sb.Append(Text, lastIndex, Text.Length - lastIndex);
        }
    }

    [GeneratedRegex(@"(?<nested>\{\{(\w+)\}\})|(?<normal>\{(\w+)\})")]
    private static partial Regex ParameterRegex();
}
