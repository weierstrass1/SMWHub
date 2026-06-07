using StateMachine;
using Validations;
namespace SMWHubValidations;

[RequiresStateVariable("Filepath", typeof(string))]
public sealed class ValidateFileExists(IHaveState ctx) : Validator(ctx)
{
    public override ValidationResult Validate(IHaveState ctx)
    {
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        ValidationResult validationResult = new();
        if (!File.Exists(filepath))
            validationResult.AddError(LogMessageTypeKeys.RESOURCE_NOT_FOUND, new()
            {
                {"file", filepath }
            });
        return validationResult;
    }
}
