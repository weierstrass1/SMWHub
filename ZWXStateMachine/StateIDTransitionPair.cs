using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

/// <summary>
/// Represents a transition from the current state to another state.
/// </summary>
/// <typeparam name="T">
/// An enumeration that uniquely identifies a state.
/// </typeparam>
/// <remarks>
/// Initializes a new transition definition.
/// </remarks>
/// <param name="idToTransition">
/// The identifier of the destination state.
/// </param>
/// <param name="transition">
/// The transition condition.
/// </param>
public class StateIDTransitionPair<T>(T idToTransition, ITransition transition, int priority = 0) where T : struct, Enum
{
    /// <summary>
    /// Used to sort the transition list for execution order.
    /// Lower values execute earlier than higher values.
    /// By default it is 0.
    /// </summary>
    public readonly int Priority = priority; 
    /// <summary>
    /// Gets or sets the identifier of the destination state.
    /// </summary>
    public readonly T IDToTransition = idToTransition;
    /// <summary>
    /// Gets or sets the transition condition.
    /// </summary>
    public readonly ITransition Transition = transition;

    /// <summary>
    /// Deconstructs the pair into its destination state identifier and transition.
    /// </summary>
    public void Deconstruct(out T idToTransition, out ITransition transition)
    {
        idToTransition = IDToTransition;
        transition = Transition;
    }
}