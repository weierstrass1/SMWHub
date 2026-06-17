using StateMachine.Interfaces;

namespace StateMachine.StateLogics;

public class StateLogic<T>(T id, bool executeLoopRightAfterTransition = true) : IStateLogic<T> where T : struct, Enum
{
    public T ID { get; } = id;
    public bool ExecuteLoopRightAfterTransition { get; set; } = executeLoopRightAfterTransition;
}
