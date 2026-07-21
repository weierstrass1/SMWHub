using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine.StateBehaviours;

public class DelegateStateBehaviourEnter(Action<StateData> enter) : IStateBehaviourEnter
{
    private readonly Action<StateData> _enter = enter;
    public bool ExecuteUpdateRightAfterTransition => false;
    public void Enter(StateData stateData)
    {
        _enter(stateData);
    }
}
