
namespace Validations;
public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = [];
    public void AddLine(int line, string lineContent)
    {
        foreach (var error in Errors)
        {
            error.addParameter("line", line.ToString());
            error.addParameter("lineContent", lineContent);
        }
    }
    public void AddFile(string file)
    {
        foreach (var error in Errors)
        {
            error.addParameter("file", file);
        }
    }
    public void AddError(string messageTypeKey)
    {
        Errors.Add(new ValidationError(messageTypeKey));
    }
    public void AddError(string messageTypeKey, Dictionary<string, string> parameters)
    {
        Errors.Add(new ValidationError(messageTypeKey, parameters));
    }
    public void Merge(ValidationResult other)
    {
        Errors.AddRange(other.Errors);
    }
    public static implicit operator bool(ValidationResult validationResult)
    {
        return validationResult.IsValid;
    }
}
public sealed class ValidationError
{
    public string this[string index] { get => _parameters[index]; }
    public string MessageTypeKey { get; private set; }
    private readonly Dictionary<string, string> _parameters;
    public ValidationError(string messageTypeKey)
    {
        MessageTypeKey = messageTypeKey;
        _parameters = new Dictionary<string, string>();
    }
    public ValidationError(string messageTypeKey, Dictionary<string, string> parameters)
    {
        MessageTypeKey = messageTypeKey;
        _parameters = parameters;
    }
    internal void addParameter(string key, string value)
    {
        _parameters[key] = value;
    }
}
