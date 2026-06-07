namespace Validations.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RequiresStateVariableAttribute(string variableName, Type? expectedType = null) : Attribute
{
    public readonly VariableValidator Validator = new(variableName, expectedType);
}
