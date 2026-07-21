namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Defines the requirements for a state variable.
///
/// This interface is primarily intended to be implemented by attributes
/// that declare the expected existence and type of a state variable.
/// </summary>
public interface IVariableValidator
{
    /// <summary>
    /// Gets the name of the required state variable.
    /// </summary>
    public string VariableName { get; }
#nullable enable
    /// <summary>
    /// Gets the expected type of the variable, or <see langword="null"/>
    /// if no type validation is required.
    /// </summary>
    public Type? ExpectedType { get; }
}
