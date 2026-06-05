using FormatReadLibrary.Logging;
using StateMachine;
namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateFileExists() : Validator()
{
    private readonly static VariableValidator _variableValidator = new("Filepath", typeof(string));
    public override ValidationResult Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var filepath = state.Get<string>("Filepath")!;
        return Validate(filepath);
    }
    public ValidationResult Validate(string filepath)
    {
        ValidationResult validationResult = new();
        if (!File.Exists(filepath))
            validationResult.AddError(LogMessageTypeKeys.RESOURCE_NOT_FOUND, new()
            {
                {"file", filepath }
            });
        return validationResult;
    }
}
