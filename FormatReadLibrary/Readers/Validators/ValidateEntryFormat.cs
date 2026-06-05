using FormatReadLibrary.Logging;
using FormatReadLibrary.Readers.Enumerators;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryFormat : Validator
{
    public ValidateEntryFormat(IHaveState context, string variableName = "Match") : base()
    {
        VariableValidator validator = new(variableName, typeof(Match));
        validator.Validate(context);
    }
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult validationResult = new();
        Match match = ctx.State.Get<Match>("Match")!;
        if (!match.Success)
            validationResult.AddError(ValidatorMessagetypeKeys.INVALID_ENTRY_FORMAT);

        return validationResult;
    }
}
