using FormatReadLibrary.Logging;
using FormatReadLibrary.Readers.Enumerators;

namespace FormatReadLibrary.Readers.Validators;

[RequiresStateVariable("Start", typeof(int))]
[RequiresStateVariable("End", typeof(int))]
public sealed class ValidateGPSBlockLine(IHaveState context) : Validator(context)
{
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult validationResult = new();
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
