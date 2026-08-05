
namespace Validations;

public sealed class ValidationResult(ValidationContext? context = null)
{
    public ValidationContext? Context { get; set; } = context;
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = [];
    public void AddError(ValidationContext context, string messageTypeKey)
    {
        Errors.Add(new ValidationError(context, messageTypeKey));
    }
    public void AddError(ValidationContext context, string messageTypeKey, Dictionary<string, string> parameters)
    {
        Errors.Add(new ValidationError(context, messageTypeKey, parameters));
    }
    public void AddError(string messageTypeKey)
    {
        if (Context == null)
            throw new NullReferenceException();
        AddError(Context, messageTypeKey);
    }
    public void AddError(string messageTypeKey, Dictionary<string, string> parameters)
    {
        if (Context == null)
            throw new NullReferenceException(nameof(Context));
        AddError(Context, messageTypeKey, parameters);
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
