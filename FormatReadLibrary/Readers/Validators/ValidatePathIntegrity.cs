using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("Filepath", typeof(string))]
public class ValidatePathIntegrity(IHaveState ctx) : Validator(ctx)
{
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult validationResult = new();
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
