using FormatReadLibrary.Readers.Enumerators;

namespace FormatReadLibrary.Readers.Validators;

public enum TableValueSize
{
    db,
    dw,
    dl,
    dd
}
internal class ValidateTableValueSize(IHaveState context, FileEnumeratorWithLog fileEnumerator, TableValueSize valueSize) : Validator(context)
{
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly TableValueSize _valueSize = valueSize;
    public override bool Validate(IHaveState ctx)
    {
        string name = _valueSize.ToString();
        if (_fileEnumerator.Current.Length > 1 && _fileEnumerator.Current[0..2] != name)
        {
            _fileEnumerator.AddSyntaxErrorLog($"Should use {name}");
            return false;
        }
        return true;
    }
}
