namespace FormatReadLibrary.Readers.Validators;

public interface IVariableValidator
{
    public string VariableName { get; }
    public Type? ExpectedType { get; }
}
