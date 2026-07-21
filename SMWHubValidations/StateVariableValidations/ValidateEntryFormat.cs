using System.Text.RegularExpressions;
using Validations;
using Validations.Interfaces;
using ZWXStateMachine;

namespace SMWHubValidations.StateVariableValidations;

public sealed class ValidateEntryFormat : VariableValidation
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
        Match match = ctx.StateData.Get<Match>(_variableName)!;
        ValidationResult validationResult = new(ctx.Context);
        if (!match.Success)
            validationResult.AddError(StateVariableMessageTypeKeys.INVALID_ENTRY_FORMAT);

        return validationResult;
    }
}
