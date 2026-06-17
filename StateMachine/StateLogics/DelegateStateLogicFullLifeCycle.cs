using StateMachine.Interfaces;

namespace StateMachine;

public class DelegateStateLogicFullLifeCycle<T>(T id, Action<State> startDelegate, Action<State> updateDelegate, Action<State> exitDelegate, bool executeLoopRightAfterTransition = true) : IStateLogicFullLifeCycle<T> where T : struct, Enum
{
    public T ID { get; } = id;
    public bool ExecuteLoopRightAfterTransition { get; set; } = executeLoopRightAfterTransition;
    private readonly Action<State> _startDelegate = startDelegate;
    private readonly Action<State> _updateDelegate = updateDelegate;
    private readonly Action<State> _exitDelegate = exitDelegate;
    public void Start(State state)
    {
        _startDelegate(state);
    }
    public void Update(State state)
    {
        _updateDelegate(state);
    }
    public void Exit(State state)
    {
        _exitDelegate(state);
    }
}
