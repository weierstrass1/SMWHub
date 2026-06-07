using LogRegister.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace LogRegister;

public sealed partial class LogMessageType
{
    public string Key { get; private set; }
    public string Text { get; private set; }
    public ILogCategory Category { get; private init; }
    private readonly LogSpan _prefixSpan;
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
    public static LogMessageType FromDTO(LogRegisterSystem log, string key, LogMessageTypeDTO messageDTO)
    {
        ILogCategory? cat = log.GetCategory(messageDTO.Category);
        if (cat == null || cat.GetType().Name == typeof(UnknownCategory).Name)
            throw new InvalidOperationException($"Error in Message \"{key}\": Category {messageDTO.Category} doesn't exist.");
        
        LogMessageType reg = new(key, messageDTO.Message, cat);

        return reg;
    }
    public void Validate(ILoggingEntry entry)
    {
        var variables = _variables
            .Where(v => v.Value.Type == SpanType.Parameter)
            .ToDictionary(v => v.Key, v => v.Value);

        var unexpectedParam = entry.Parameters.Keys
            .FirstOrDefault(p => !variables.ContainsKey(p));
        if (unexpectedParam != null)
            throw new InvalidOperationException($"Parameter {unexpectedParam} is invalid.");

        var missingParam = variables.Keys.FirstOrDefault(v => !entry.Parameters.ContainsKey(v));
        if(missingParam != null)
            throw new InvalidOperationException($"Parameter {missingParam} is missing.");
        if (entry is not ILoggingEntryWithNestedMessage entryWithNestedMessage)
            return;

        var nestedLogs = _variables
            .Where(v => v.Value.Type == SpanType.NestedMessage)
            .ToDictionary(v => v.Key, v => v.Value);

        var unexpectedLog = entryWithNestedMessage.NestedEntries.Keys
            .FirstOrDefault(p => !nestedLogs.ContainsKey(p));
        if (unexpectedLog != null)
            throw new InvalidOperationException($"Nested Log {unexpectedLog} is invalid.");
        var missingLog = nestedLogs.Keys.FirstOrDefault(v => !entryWithNestedMessage.NestedEntries.ContainsKey(v));
        if (missingLog != null)
            throw new InvalidOperationException($"Parameter {missingLog} is missing.");
    }
    public LogRenderResult GetMessage(LogRegisterSystem log, ILoggingEntry logEntry)
    {
        Dictionary<string, LogRenderResult> nestedMessages = getNestedResults(log, logEntry);
        var sb = new StringBuilder();
        int lastIndex = 0;

        List<LogSpan> spans = [_prefixSpan];

        foreach ((string key, LogSpan span) in _variables)
        {
            appendTextBeforeSpan(sb, lastIndex, span);

            if (span.Type == SpanType.NestedMessage && nestedMessages.TryGetValue(key, out LogRenderResult? nestedResult))
                appendNestedResult(sb, spans, nestedResult);
            else
                appendRegularSpan(logEntry, sb, spans, key);
            lastIndex = span.Start + span.Length;
        }
        appendTextAfterLastSpan(sb, lastIndex);
        return new(Category, sb.ToString(), spans.AsReadOnly());
    }
    public override string ToString()
    {
        return Text;
    }
    private static Dictionary<string, LogRenderResult> getNestedResults(LogRegisterSystem log, ILoggingEntry logEntry)
    {
        Dictionary<string, LogRenderResult> nestedMessages = [];
        if (logEntry is ILoggingEntryWithNestedMessage loggingEntryWithNestedMessage)
        {
            nestedMessages = loggingEntryWithNestedMessage
                .NestedEntries
                .ToDictionary(ne => ne.Key, 
                    ne => log.GetMessageType(ne.Value.MessageTypeKey).GetMessage(log, ne.Value));
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
    private static void appendNestedResult(StringBuilder sb, List<LogSpan> spans, LogRenderResult nestedResult)
    {
        nestedResult.RemoveOfType(SpanType.Prefix);
        nestedResult.DisplaceAll(sb.Length);
        foreach (var s in nestedResult.Spans)
            spans.Add(s);
        sb.Append(nestedResult.Text);
    }
    private void appendRegularSpan(ILoggingEntry logEntry, StringBuilder sb, List<LogSpan> spans, string key)
    {
        if (!logEntry.Parameters.TryGetValue(key, out string? value))
            value = $"{{{key}}}";

        int start = sb.Length;

        sb.Append(value);

        spans.Add(new()
        {
            Category = Category,
            Start = start,
            Length = value.Length,
            Type = SpanType.Parameter
        });
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
