using SMWHubEnumerators;
using Validations;
using Validations.Interfaces;

namespace SMWHubValidations;

public class ValidateIfHasNext(FileEnumerator fileEnumerator) : Validator()
{
    private readonly FileEnumerator _fileEnumerator = fileEnumerator;
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        if (_fileEnumerator.IsLastLine)
            validationResult.AddError(ValidatorMessagetypeKeys.EOF);
        return validationResult;
    }
    public void MoveToTheNextNotEmptyLine()
    {
        while (_fileEnumerator.MoveNext() && string.IsNullOrWhiteSpace(_fileEnumerator.Current))
        {
        }
    }
}
