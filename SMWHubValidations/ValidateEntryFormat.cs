using System.Text.RegularExpressions;
using Validations;

namespace SMWHubValidations;
public sealed class ValidateEntryFormat : Validator
{
    private readonly string _variableName;
    public ValidateEntryFormat(IHaveState context, string variableName = "Match") : base()
    {
        _variableName = variableName;
        VariableValidator validator = new(variableName, typeof(Match));
        validator.Validate(context);
    }
    public override ValidationResult Validate(IHaveState ctx)
    {
        Match match = ctx.State.Get<Match>(_variableName)!;
        ValidationResult validationResult = new();
        if (!match.Success)
            validationResult.AddError(ValidatorMessagetypeKeys.INVALID_ENTRY_FORMAT);

        return validationResult;
    }
}
