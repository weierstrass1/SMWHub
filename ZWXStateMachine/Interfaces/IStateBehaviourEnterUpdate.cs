namespace ZWXStateMachine.Interfaces;

/// <summary>
/// Represents a state logic that supports both enter and update callbacks.
/// </summary>
public interface IStateBehaviourEnterUpdate : IStateBehaviourEnter, IStateBehaviourUpdate
{
}
