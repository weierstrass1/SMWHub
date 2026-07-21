using Validations;
using Validations.Interfaces;
using ZWXStateMachine;
using ZWXStateMachine.Attributes;

namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("ID", typeof(int))]
public sealed class ValidateEntryID(IValidationState ctx, int maxID = 255) : VariableValidation(ctx)
{
    private readonly int _maxID = maxID;
    public override ValidationResult Validate(IValidationState ctx)
    {
        StateData state = ctx.StateData;
        var id = state.Get<int>("ID")!;
        ValidationResult validationResult = new(ctx.Context);
        if (id > _maxID)
            validationResult.AddError(StateVariableMessageTypeKeys.ID_SURPASS_LIMIT, new() {
                { "id", id.ToString("X2") },
                { "maxID", _maxID.ToString("X2")}
            });

        return validationResult;
    }
}
