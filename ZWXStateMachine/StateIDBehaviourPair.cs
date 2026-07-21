using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Represents the association between a state identifier and its logic.
/// </summary>
/// <typeparam name="T">
/// An enumeration that uniquely identifies a state.
/// </typeparam>
/// <remarks>
/// Initializes a new state identifier and logic association.
/// </remarks>
/// <param name="id">
/// The state identifier.
/// </param>
/// <param name="stateLogic">
/// The logic associated with the state.
/// </param>
public class StateIDBehaviourPair<T>(T id, IStateBahaviour stateLogic) where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the state identifier.
    /// </summary>
    public readonly T ID = id;
    /// <summary>
    /// Gets or sets the logic associated with the state.
    /// </summary>
    public readonly IStateBahaviour StateLogic = stateLogic;

    /// <summary>
    /// Deconstructs the pair into its identifier and state logic.
    /// </summary>
    public void Deconstruct(out T id, out IStateBahaviour stateLogic)
    {
        id = ID;
        stateLogic = StateLogic;
    }
    public static implicit operator StateIDBehaviourPair<T>((T, IStateBahaviour) tuple)
    {
        return new(tuple.Item1, tuple.Item2);
    }
}