using System.Text.RegularExpressions;
using Validations;

namespace SMWHubValidations;
public sealed class ValidateEntryFormat : Validator
{
    private readonly string _variableName;
    public ValidateEntryFormat(IValidationState context, string variableName = "Match") : base()
    {
        _variableName = variableName;
        VariableValidator validator = new(variableName, typeof(Match));
        validator.Validate(context);
    }
    public override ValidationResult Validate(IValidationState ctx)
    {
        Match match = ctx.State.Get<Match>(_variableName)!;
        ValidationResult validationResult = new(ctx.Context);
        if (!match.Success)
            validationResult.AddError(ValidatorMessagetypeKeys.INVALID_ENTRY_FORMAT);

        return validationResult;
    }
}
