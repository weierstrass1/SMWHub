using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine.StateLogics;

public class EmptyStateBehaviour : IStateBahaviour
{
    public bool ExecuteUpdateRightAfterTransition => false;
    public EmptyStateBehaviour()
    {
    }
}
