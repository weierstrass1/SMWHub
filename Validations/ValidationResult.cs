
namespace Validations;
public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = [];
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
    public string this[string index] { get => Parameters[index]; }
    public string MessageTypeKey { get; private set; }
    public IReadOnlyDictionary<string, string> Parameters { get; private set; }
    public ValidationError(string messageTypeKey)
    {
        MessageTypeKey = messageTypeKey;
        Parameters = new Dictionary<string, string>().AsReadOnly();
    }
    public ValidationError(string messageTypeKey, Dictionary<string, string> parameters)
    {
        MessageTypeKey = messageTypeKey;
        Parameters = parameters.AsReadOnly();
    }
}
