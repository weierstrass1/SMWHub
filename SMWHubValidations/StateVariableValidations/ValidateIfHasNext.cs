using SMWHubEnumerators;
using Validations;
using Validations.Interfaces;

namespace SMWHubValidations.StateVariableValidations;

public class ValidateIfHasNext(FileLineEnumerator fileEnumerator) : VariableValidation()
{
    private readonly FileLineEnumerator _fileEnumerator = fileEnumerator;
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        if (_fileEnumerator.IsLastLine)
            validationResult.AddError(StateVariableMessageTypeKeys.EOF);
        return validationResult;
    }
    public void MoveToTheNextNotEmptyLine()
    {
        while (_fileEnumerator.MoveNext() && string.IsNullOrWhiteSpace(_fileEnumerator.Current))
        {
        }
    }
}
