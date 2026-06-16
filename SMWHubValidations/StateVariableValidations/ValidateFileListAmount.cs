using FormatLibrary.Entries;
using Validations;
using Validations.Attributes;
using Validations.Interfaces;

namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Filelist", typeof(FilePath[]))]
public class ValidateFileListAmount(IValidationState ctx, bool allowedMultiline) : Validator(ctx)
{
    private readonly bool _allowedMultiline = allowedMultiline;
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult result = new(ctx.Context);
        var filepath = ctx.State.Get<FilePath[]>("Filelist");
        if (filepath == null || filepath.Length == 0)
            result.AddError(StateVariableMessageTypeKeys.MISSING_FILEPATH);
        else if (_allowedMultiline && filepath!.Length != 1)
            result.AddError(StateVariableMessageTypeKeys.MORE_THAN_1_FILEPATH);
        return result;
    }
}
