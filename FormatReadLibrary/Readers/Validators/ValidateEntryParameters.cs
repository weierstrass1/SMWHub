using StateMachine;

namespace FormatReadLibrary.Readers.Validators;
[RequiresStateVariable("Parameters", typeof(int[]))]
public sealed class ValidateEntryParameters(IHaveState ctx, int minLimit = 0, int maxLimit = 255, bool allowedVariables = false) : Validator(ctx)
{
    private readonly bool _allowedVariables = allowedVariables;
    private readonly int _minLimit = minLimit;
    private readonly int _maxLimit = maxLimit;
    public override ValidationResult Validate(IHaveState ctx)
    {
        State state = ctx.State;
        int[]? parameters = state.Get<int[]>("Parameters");
        ValidationResult validationResult = new();
        if (parameters == null || parameters.Length == 0)
            return validationResult;
        foreach(var par in parameters)
        {
            if (par < _minLimit || par > _maxLimit)
                validationResult.AddError(ValidatorMessagetypeKeys.PARAMETER_OUT_OF_RANGE, new()
                    {
                        {"parameter", "" },
                        {"value", $"'{par}'" },
                        {"valueHex", $"'${par:X2}'" },
                        {"minLimit", _minLimit.ToString() },
                        {"maxLimit", _maxLimit.ToString() },
                        {"minLimitHex", $"${_minLimit:X2}" },
                        {"maxLimitHex", $"${_maxLimit:X2}" },
                    });
        }
        if (_allowedVariables)
            validationResult.AddError(new(ValidatorMessagetypeKeys.LIST_DOESNT_ALLOW_PARAMETERS));

        return validationResult;
    }
}
