namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state variable that stores a value.
/// </summary>
public interface IStateVariable
{
#nullable enable
    /// <summary>
    /// Gets or sets the value stored by this variable.
    /// </summary>
    public object? Value { get; set; }
}
/// <summary>
/// Represents a strongly typed state variable.
/// </summary>
/// <typeparam name="T">
/// The type of the stored value.
/// </typeparam>
public interface IStateVariable<T> : IStateVariable
{
    /// <summary>
    /// Gets or sets the value stored by this variable.
    /// </summary>
    public new T? Value { get; set; }
}
