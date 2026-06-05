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
            _variables.Add(match.Value[1..^1], new()
            {
                Category = Category,
                Start = match.Index,
                Length = match.Length,
                Type = SpanType.Parameter
            });
        }
    }
    public static LogMessageType FromDTO(LogRegisterSystem log, LogMessageTypeDTO messageDTO)
    {
        ILogCategory? cat = log.GetCategory(messageDTO.Category) ?? 
            throw new InvalidOperationException($"Error in Message \"{messageDTO.MessageType}\": Category {messageDTO.Category} doesn't exist.");
        LogMessageType reg = new(messageDTO.MessageType, messageDTO.Message, cat);

        return reg;
    }
    public void Validate(ILoggingEntry entry)
    {
        foreach (var param in entry.Parameters.Keys)
        {
            if (!_variables.ContainsKey(param))
                throw new InvalidOperationException($"Parameter {param} is invalid.");
        }
        foreach(var vt in _variables.Keys)
        {
            if (!entry.Parameters.ContainsKey(vt))
                throw new InvalidOperationException($"Parameter {vt} is missing.");
        }
    }
    public LogRenderResult GetMessage(ILoggingEntry logEntry)
    {
        Validate(logEntry);
        var sb = new StringBuilder();
        int lastIndex = 0;

        List<LogSpan> spans = [_prefixSpan];

        foreach ((string key, LogSpan span) in _variables)
        {
            if (span.Start > lastIndex)
            {
                sb.Append(Text, lastIndex, span.Start - lastIndex);
            }

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

            lastIndex = span.Start + span.Length;
        }
        if (lastIndex < Text.Length)
        {
            sb.Append(Text, lastIndex, Text.Length - lastIndex);
        }
        return new()
        {
            Category = Category,
            Spans = spans.AsReadOnly(),
            Text = sb.ToString()
        };
    }
    [GeneratedRegex(@"\{(\w+)\}")]
    private static partial Regex ParameterRegex();
}
