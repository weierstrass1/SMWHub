namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Defines the common configuration shared by all state logic implementations.
/// </summary>
public interface IStateBahaviour
{
    /// <summary>
    /// Gets whether the state machine should execute the update
    /// immediately after transitioning into this state.
    /// </summary>
    public bool ExecuteUpdateRightAfterTransition { get; }
}
