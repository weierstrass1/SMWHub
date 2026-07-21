namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state logic that is notified when a state is entered.
/// </summary>
public interface IStateBehaviourEnter : IStateBahaviour
{
    /// <summary>
    /// Called when the state becomes active.
    /// </summary>
    /// <param name="state">
    /// The shared runtime state.
    /// </param>
    public void Enter(StateData state);
}
