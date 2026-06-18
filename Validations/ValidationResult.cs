
namespace Validations;

public sealed class ValidationResult(ValidationContext? context = null)
{
    public ValidationContext? Context { get; set; } = context;
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = [];
    public void AddError(string messageTypeKey)
    {
        if (Context == null)
            throw new NullReferenceException();
        Errors.Add(new ValidationError(Context, messageTypeKey));
    }
    public void AddError(string messageTypeKey, Dictionary<string, string> parameters)
    {
        if (Context == null)
            throw new NullReferenceException(nameof(Context));
        Errors.Add(new ValidationError(Context, messageTypeKey, parameters));
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
