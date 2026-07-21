namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state logic that supports the complete state lifecycle:
/// enter, update, and exit.
/// </summary>
public interface IStateBehaviourFullLifeCycle : IStateBehaviourEnter, IStateBehaviourUpdate, IStateBehaviourExit
{
}
