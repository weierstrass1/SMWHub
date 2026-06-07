namespace Validations;

public sealed class ValidationError
{
    public string this[string index] { get => _parameters[index]; }
    public string MessageTypeKey { get; private set; }
    private readonly Dictionary<string, string> _parameters;
    public readonly ValidationContext Context;
    public ValidationError(ValidationContext context, string messageTypeKey)
    {
        MessageTypeKey = messageTypeKey;
        _parameters = [];
        Context = context;
    }
    public ValidationError(ValidationContext context, string messageTypeKey, Dictionary<string, string> parameters)
    {
        MessageTypeKey = messageTypeKey;
        _parameters = parameters;
        Context = context;
    }
}
