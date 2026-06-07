using Validations;
using Validations.Attributes;
using Validations.Interfaces;

namespace SMWHubValidations;

[RequiresStateVariable("Start", typeof(int))]
[RequiresStateVariable("End", typeof(int))]
public sealed class ValidateGPSBlockLine(IValidationState context) : Validator(context)
{
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        int start = ctx.State.Get<int>("Start");
        int end = ctx.State.Get<int>("End");
        if ($"{end:X2}"[..^1] != $"{start:X2}"[..^1])
            validationResult.AddError(ValidatorMessagetypeKeys.INVALID_GPS_BLOCK_LINE, new()
            {
                {"start", $"{start:X4}" },
                {"end", $"{end:X4}" }
            });
        return validationResult;
    }
}
