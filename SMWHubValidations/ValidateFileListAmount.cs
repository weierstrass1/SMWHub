using Validations;

namespace SMWHubValidations;

[RequiresStateVariable("Filelist", typeof(FilePath[]))]
public class ValidateFileListAmount(IValidationState ctx, bool allowedMultiline) : Validator(ctx)
{
    private readonly bool _allowedMultiline = allowedMultiline;
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult result = new(ctx.Context);
        var filepath = ctx.State.Get<FilePath[]>("Filelist");
        if (filepath == null || filepath.Length == 0)
            result.AddError(ValidatorMessagetypeKeys.MISSING_FILEPATH);
        else if(_allowedMultiline && filepath!.Length != 1)
            result.AddError(ValidatorMessagetypeKeys.MORE_THAN_1_FILEPATH);
        return result;
    }
}
public record FilePath(string Path, int[] Values);
