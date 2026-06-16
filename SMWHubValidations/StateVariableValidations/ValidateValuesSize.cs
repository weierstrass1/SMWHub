using Validations;
using Validations.Attributes;
using Validations.Interfaces;

namespace SMWHubValidations.StateVariableValidations;

[RequiresStateVariable("Values", typeof(int[]))]
public class ValidateValuesSize : Validator
{
    private readonly uint _minSize;
    private readonly uint _maxSize;
    public ValidateValuesSize(IValidationState context, uint minSize, uint maxSize) : base(context)
    {
        if (minSize > maxSize)
            ArgumentOutOfRangeException.ThrowIfGreaterThan(minSize, maxSize, nameof(minSize));

        _minSize = minSize;
        _maxSize = maxSize;
    }
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        var values = ctx.State.Get<int[]>("Values");
        if (values == null && _minSize == 0 ||
            values != null && values.Length >= _minSize && values.Length <= _maxSize)
            return validationResult;

        if (_minSize != _maxSize)
        {
            validationResult.AddError(StateVariableMessageTypeKeys.VALUES_SET_ALLOW_BETWEEN_N_AND_M_VALUES, new()
            {
                { "minSize", _minSize.ToString()},
                { "maxSize", _maxSize.ToString() }
            });
            return validationResult;
        }
        if (_minSize == 1)
            validationResult.AddError(StateVariableMessageTypeKeys.VALUES_SET_ALLOW_1_VALUE);
        else
            validationResult.AddError(StateVariableMessageTypeKeys.VALUES_SET_ALLOW_N_VALUES, new()
            {
                { "size", _minSize.ToString()},
            });
        return validationResult;
    }
}
