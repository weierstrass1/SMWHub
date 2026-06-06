using FormatReadLibrary.Readers.Validators;

namespace FormatReadLibrary.Readers.StateVariables
{
    public interface ISelfValidatedStateVariable
    {
        public ValidationResult GetFrom(string text);
    }
}
