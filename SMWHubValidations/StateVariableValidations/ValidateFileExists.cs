using SMWHubValidations.FormatValidations;
using StateMachine;
using StateMachine.Attributes;
using Validations;
using Validations.Interfaces;
namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Filepath", typeof(string))]
public sealed class ValidateFileExists(IValidationState ctx) : VariableValidation(ctx)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        State state = ctx.State;
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
