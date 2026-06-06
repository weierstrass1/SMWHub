using FormatReadLibrary.Logging;
using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Readers.Validators;

namespace FormatReadLibrary.Readers.ParsingContexts;

public abstract class ParsingContext : StateValidator
{
    public FileEnumeratorWithLog FileEnumerator { get; private set; }
    public ParsingContext(FileEnumeratorWithLog fileEnumerator)
    {
        FileEnumerator = fileEnumerator;
    }
    public abstract bool ProcessEntry();
    protected override ValidationResult getSelfValidatedVariables(string entry)
    {
        return logValidationResult(base.getSelfValidatedVariables(entry));
    }
    protected override ValidationResult validate()
    {
        return logValidationResult(base.validate());
    }
    private ValidationResult logValidationResult(ValidationResult result)
    {
        if (!result)
            ValidatorLogAdapter.LogValidatorResult(FileEnumerator, result);

        return result;
    }
}
