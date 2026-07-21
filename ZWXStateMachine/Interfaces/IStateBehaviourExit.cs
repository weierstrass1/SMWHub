namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state logic that is notified when a state is exited.
/// </summary>
public interface IStateBehaviourExit : IStateBahaviour
{
    /// <summary>
    /// Called before the state is exited.
    /// </summary>
    /// <param name="state">
    /// The shared runtime state.
    /// </param>
    public void Exit(StateData state);
}
