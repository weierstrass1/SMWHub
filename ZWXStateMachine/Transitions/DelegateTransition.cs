using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine.Transitions;

public class DelegateTransition(Func<StateData, bool> mustTransition) : ITransition
{
    private readonly Func<StateData, bool> _mustTransition = mustTransition;
    public bool MustTransition(StateData stateData)
    {
        return _mustTransition(stateData);
    }
}
