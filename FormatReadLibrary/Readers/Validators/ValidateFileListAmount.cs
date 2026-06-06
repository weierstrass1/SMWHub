using FormatReadLibrary.Readers.StateVariables;

namespace FormatReadLibrary.Readers.Validators;

[RequiresStateVariable("Filelist", typeof(FilePath[]))]
public class ValidateFileListAmount(IHaveState ctx, bool allowedMultiline) : Validator(ctx)
{
    private readonly bool _allowedMultiline = allowedMultiline;
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult result = new();
        var filepath = ctx.State.Get<FilePath[]>("Filelist");
        if (filepath == null || filepath.Length == 0)
            result.AddError(ValidatorMessagetypeKeys.MISSING_FILEPATH);
        else if(_allowedMultiline && filepath!.Length != 1)
            result.AddError(ValidatorMessagetypeKeys.MORE_THAN_1_FILEPATH);
        return result;
    }
}
