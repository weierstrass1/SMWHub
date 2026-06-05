using FormatReadLibrary.Logging;
using FormatReadLibrary.Readers.Enumerators;
using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
public sealed class ValidateEntryID(int maxID = 255) : Validator()
{
    private static readonly VariableValidator _variableValidator = new("ID", typeof(int?));
    private readonly int _maxID = maxID;
    public override ValidationResult Validate(IHaveState ctx)
    {
        _variableValidator.Validate(ctx);
        State state = ctx.State;
        var id = state.Get<int>("ID")!;
        return Validate(id);
    }
    public ValidationResult Validate(int id)
    {
        ValidationResult validationResult = new();
        if (id > _maxID)
            validationResult.AddError(ValidatorMessagetypeKeys.ID_SURPASS_LIMIT, new() { 
                { "id", id.ToString("X2") },
                { "maxID", _maxID.ToString("X2")}
            });

        return validationResult;
    }
}
