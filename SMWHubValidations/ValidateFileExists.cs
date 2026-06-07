using StateMachine;
using Validations;
using Validations.Attributes;
using Validations.Interfaces;
namespace SMWHubValidations;

[RequiresStateVariable("Filepath", typeof(string))]
public sealed class ValidateFileExists(IValidationState ctx) : Validator(ctx)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        ValidationResult validationResult = new(ctx.Context);
        if (!File.Exists(filepath))
            validationResult.AddError(ValidatorMessagetypeKeys.RESOURCE_NOT_FOUND, new()
            {
                {"file", filepath }
            });
        return validationResult;
    }
}
