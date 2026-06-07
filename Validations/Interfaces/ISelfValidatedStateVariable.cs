namespace Validations.Interfaces;

public interface ISelfValidatedStateVariable
{
    public ValidationResult GetFrom(ValidationContext context, string text);
}
