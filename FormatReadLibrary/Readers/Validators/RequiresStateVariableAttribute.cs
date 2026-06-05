namespace FormatReadLibrary.Readers.Validators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RequiresStateVariableAttribute : Attribute
{
    public readonly VariableValidator Validator;
    public RequiresStateVariableAttribute(string variableName, Type? expectedType = null)
    {
        Validator = new(variableName, expectedType);
    }
}
