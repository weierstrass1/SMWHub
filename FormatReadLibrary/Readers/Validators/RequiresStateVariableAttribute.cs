namespace FormatReadLibrary.Readers.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequiresStateVariableAttribute : Attribute
    {
        public string VariableName { get; }
        public Type? ExpectedType { get; }
        public RequiresStateVariableAttribute(string variableName, Type? expectedType = null)
        {
            VariableName = variableName;
            ExpectedType = expectedType;
        }
    }
}
