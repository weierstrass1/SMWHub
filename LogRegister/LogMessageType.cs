using Microsoft.Win32;
using System.Text;
using System.Text.RegularExpressions;

namespace LogRegister;

public sealed partial class LogMessageType(string type, string text, IDictionary<string, LogMessageVariableType> variableTypes)
{
    public required ILogCategory Category { get; init; }
    public string Type { get; private set; } = type;
    public string Text { get; private set; } = text;
    private readonly IDictionary<string, LogMessageVariableType> _variableTypes = variableTypes;
    public static LogMessageType FromDTO(LogRegisterSystem log, LogMessageTypeDTO messageDTO)
    {
        ILogCategory? cat = log.GetCategory(messageDTO.Category);
        if (cat == null)
            throw new InvalidOperationException($"Error in Message \"{messageDTO.MessageType}\": Category {messageDTO.Category} doesn't exist.");

        IDictionary<string, LogMessageVariableType> vars = messageDTO
            .variables
            .Select(v =>
            {
                return log.CreateVariableType(v.Name, v.Type);
            })
            .ToDictionary(v => v.Name, v => v);
        LogMessageType reg = new(messageDTO.MessageType, messageDTO.Message, vars)
        {
            Category = cat
        };

        return reg;
    }
    public void Validate(ILoggingRegister register)
    {
        foreach (var param in register.Parameters.Values)
        {
            if (!_variableTypes.ContainsKey(param.Name))
                throw new InvalidOperationException($"Parameter {param.Name} is invalid.");
        }
        foreach(var vt in _variableTypes.Values)
        {
            if (!register.Parameters.ContainsKey(vt.Name))
                throw new InvalidOperationException($"Parameter {vt.Name} of type {vt.VariableType.Name} is missing.");
            vt.Validate(register.Parameters[vt.Name]);
        }
    }
    public LogRenderResult GetMessage(ILoggingRegister logRegister)
    {
        Validate(logRegister);
        var sb = new StringBuilder();
        string text = $"[{Type}]: {Text}";
        int lastIndex = 0;
        string replace;
        List<LogSpan> spans = [new()
            {
                Category = Category,
                Start = 0,
                Length = Type.Length + 4,
                Type = SpanType.Prefix
            }];

        foreach (Match match in ParameterRegex().Matches(Text))
        {
            if (match.Index > lastIndex)
            {
                sb.Append(text, lastIndex, match.Index - lastIndex);
            }

            string key = match.Groups[1].Value;
            logRegister.Parameters.TryGetValue(key, out LogMessageParameter? value);

            replace = value == null ? 
                match.Value : 
                value.Value.ToString()!;

            int start = sb.Length;

            sb.Append(value);

            spans.Add(new()
            {
                Category = Category,
                Start = start,
                Length = replace.Length,
                Type = SpanType.Parameter
            });

            lastIndex = match.Index + match.Length;
        }
        if (lastIndex < text.Length)
        {
            sb.Append(text, lastIndex, text.Length - lastIndex);
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
