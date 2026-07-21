using SMWHubValidations.FormatValidations;
using Validations;
using Validations.Interfaces;
using ZWXStateMachine;
using ZWXStateMachine.Attributes;
namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Filepath", typeof(string))]
public sealed class ValidateFileExists(IValidationState ctx) : VariableValidation(ctx)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        StateData state = ctx.StateData;
        var filepath = state.Get<string>("Filepath")!;
        ValidationResult validationResult = new(ctx.Context);
        if (!File.Exists(filepath))
            validationResult.AddError(FormatErrorsMessageTypeKeys.RESOURCE_NOT_FOUND, new()
            {
                {"file", filepath }
            });
        return validationResult;
    }
}
