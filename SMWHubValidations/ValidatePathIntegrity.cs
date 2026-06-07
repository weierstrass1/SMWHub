using StateMachine;
using Validations;

namespace SMWHubValidations;
[RequiresStateVariable("Filepath", typeof(string))]
public class ValidatePathIntegrity(IValidationState ctx) : Validator(ctx)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        if (filepath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            validationResult.AddError(ValidatorMessagetypeKeys.INVALID_PATH, new()
            {
                { "path", $"'{filepath}'"}
            });
        return validationResult;
    }
}
