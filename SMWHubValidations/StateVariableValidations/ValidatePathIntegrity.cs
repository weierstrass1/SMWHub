using StateMachine;
using StateMachine.Attributes;
using Validations;
using Validations.Interfaces;

namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Filepath", typeof(string))]
public class ValidatePathIntegrity(IValidationState ctx) : VariableValidation(ctx)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        if (filepath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            validationResult.AddError(StateVariableMessageTypeKeys.INVALID_PATH, new()
            {
                { "path", $"'{filepath}'"}
            });
        return validationResult;
    }
}
