using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Represents a strongly-typed state variable.
/// </summary>
/// <typeparam name="T">
/// The type of the stored value.
/// </typeparam>
public class StateVariable<T> : IStateVariable<T>
{
#nullable enable
    public T? Value { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (T?)value; }
    public StateVariable()
    {
        Value = default;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="StateVariable{T}"/> class
    /// with the specified initial value.
    /// </summary>
    /// <param name="value">Initial value of the variable.</param>
    public StateVariable(T? value)
    {
        Value = value;
    }
}
