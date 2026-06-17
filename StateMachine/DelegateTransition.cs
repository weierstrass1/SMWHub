using StateMachine.Interfaces;

namespace StateMachine;

public class DelegateTransition(Func<State, bool> mustTransition) : ITransition
{
    private readonly Func<State, bool> _mustTransition = mustTransition;
    public bool MustTransition(State state)
    {
       return _mustTransition(state);
    }
}
