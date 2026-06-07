namespace Validations;

public interface ISelfValidatedStateVariable
{
    public ValidationResult GetFrom(string text);
}
