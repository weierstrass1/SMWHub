using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine.StateBehaviours;

public class EmptyStateBehaviour : IStateBahaviour
{
    public bool ExecuteUpdateRightAfterTransition => false;
    public EmptyStateBehaviour()
    {
    }
}
