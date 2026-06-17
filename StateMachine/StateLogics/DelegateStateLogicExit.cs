using StateMachine.Interfaces;

namespace StateMachine;

public class DelegateStateLogicExit<T>(T id, Action<State> exitDelegate) : IStateLogicExit<T> where T : struct, Enum
{
    public T ID { get; } = id;
    public bool ExecuteLoopRightAfterTransition { get; } = false;
    private readonly Action<State> _exitDelegate = exitDelegate;
    public void Exit(State state)
    {
        _exitDelegate(state);
    }
}
