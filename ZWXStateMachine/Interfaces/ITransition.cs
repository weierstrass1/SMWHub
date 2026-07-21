namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a transition condition between two states.
/// </summary>
public interface ITransition
{
    /// <summary>
    /// Determines whether the transition should be performed.
    /// </summary>
    /// <param name="stateData">
    /// The shared runtime state used to evaluate the transition.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the transition should occur; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool MustTransition(StateData stateData);
}
