using FormatReadLibrary.Logging;

namespace FormatReadLibrary.Readers.Validators;

public enum TableValueSize
{
    db,
    dw,
    dl,
    dd
}
public class ValidateTableValueSize(Func<string> getter, TableValueSize valueSize) : Validator()
{
    private readonly Func<string> _getter = getter;
    private readonly TableValueSize _valueSize = valueSize;
    public override ValidationResult Validate(IHaveState ctx)
    {
        ValidationResult validationResult = new();
        string name = _valueSize.ToString();
        string line = _getter();
        if (string.IsNullOrWhiteSpace(line) ||
            line.Length < 2 ||
            string.IsNullOrWhiteSpace(line[0..2]))
            validationResult.AddError(ValidatorMessagetypeKeys.MISSING_TABLE_INITIATOR, new()
            {
                {"initiator", name}
            });
        else if (line.Length > 1 && line[0..2] != name)
            validationResult.AddError(ValidatorMessagetypeKeys.UNEXPECTED_TABLE_INITIATOR, new()
            {
                {"unexpected",  $"'{line[0..2]}'"},
                {"initiator", name}
            });
        return validationResult;
    }
}
