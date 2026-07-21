namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents an object that exposes a <see cref="StateData"/>.
/// </summary>
public interface IHaveStateData
{
    /// <summary>
    /// Gets the state associated with this object.
    /// </summary>
    public StateData StateData { get; }
}
