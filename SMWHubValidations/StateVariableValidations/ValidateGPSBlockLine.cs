using Validations;
using Validations.Interfaces;
using ZWXStateMachine.Attributes;

namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Start", typeof(int))]
[RequiresStateVariable("End", typeof(int))]
public sealed class ValidateGPSBlockLine(IValidationState context) : VariableValidation(context)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        int start = ctx.StateData.Get<int>("Start");
        int end = ctx.StateData.Get<int>("End");
        if ($"{end:X2}"[..^1] != $"{start:X2}"[..^1])
            validationResult.AddError(StateVariableMessageTypeKeys.INVALID_GPS_BLOCK_LINE, new()
            {
                {"start", $"{start:X4}" },
                {"end", $"{end:X4}" }
            });
        return validationResult;
    }
}
