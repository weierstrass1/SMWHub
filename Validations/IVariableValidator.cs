namespace Validations;

public interface IVariableValidator
{
    public string VariableName { get; }
    public Type? ExpectedType { get; }
}
