namespace ZWXStateMachine.Attributes;

/// <summary>
/// Declares that a state logic requires a state variable to be present.
///
/// This attribute is used by the validation system to verify that the
/// required variable exists and optionally matches the expected type.
/// </summary>
/// <remarks>
/// Initializes a new instance of the
/// <see cref="RequiresStateVariableAttribute"/> class.
/// </remarks>
/// <param name="variableName">
/// The name of the required variable.
/// </param>
/// <param name="expectedType">
/// The expected variable type, or <see langword="null"/> if any type is
/// accepted.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RequiresStateVariableAttribute(string variableName, Type? expectedType = null) : Attribute
{
    /// <summary>
    /// Gets the declared variable requirement.
    /// </summary>
    public readonly VariableValidator Validator = new(variableName, expectedType);
}
