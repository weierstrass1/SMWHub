namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state logic that is updated while the state is active.
/// </summary>
public interface IStateBehaviourUpdate : IStateBahaviour
{
    /// <summary>
    /// Called during each update while the state remains active.
    /// </summary>
    /// <param name="state">
    /// The shared runtime state.
    /// </param>
    public void Update(StateData state);
}
