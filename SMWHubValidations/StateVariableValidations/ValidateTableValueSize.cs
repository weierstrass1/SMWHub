using Validations;
using Validations.Interfaces;

namespace SMWHubValidations.StateVariableValidations;

public enum TableValueSize
{
    db,
    dw,
    dl,
    dd
}
public class ValidateTableValueSize(Func<string> getter, TableValueSize valueSize) : VariableValidation()
{
    private readonly Func<string> _getter = getter;
    private readonly TableValueSize _valueSize = valueSize;
    public override ValidationResult Validate(IValidationState ctx)
    {
        ValidationResult validationResult = new(ctx.Context);
        string name = _valueSize.ToString();
        string line = _getter();
        if (string.IsNullOrWhiteSpace(line) ||
            line.Length < 2 ||
            string.IsNullOrWhiteSpace(line[0..2]))
            validationResult.AddError(StateVariableMessageTypeKeys.MISSING_TABLE_INITIATOR, new()
            {
                {"initiator", name}
            });
        else if (line.Length > 1 && line[0..2] != name)
            validationResult.AddError(StateVariableMessageTypeKeys.UNEXPECTED_TABLE_INITIATOR, new()
            {
                {"unexpected",  $"'{line[0..2]}'"},
                {"initiator", name}
            });
        return validationResult;
    }
}
