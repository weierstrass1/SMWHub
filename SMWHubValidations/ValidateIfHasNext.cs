using SMWHubEnumerators;
using Validations;

namespace SMWHubValidations;

public class ValidateIfHasNext(FileEnumeratorWithLog fileEnumerator) : Validator()
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult validationResult = new();
        if (_fileEnumerator.IsLastLine())
            validationResult.AddError(ValidatorMessagetypeKeys.EOF);
        return validationResult;
    }
    public void MoveToTheNextNotEmptyLine()
    {
        while(_fileEnumerator.MoveNext() && string.IsNullOrWhiteSpace(_fileEnumerator.Current))
        {
        }
    }
}
