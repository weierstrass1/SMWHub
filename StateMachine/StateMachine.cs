using StateMachine.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace StateMachine;

public class StateMachine<T> where T : struct, Enum
{
    public IStateLogic<T> CurrentState { get; private set; }
    private Dictionary<T, List<StateEnumTransitionPair<T>>> _transitions;
    private Dictionary<T, IStateLogic<T>> _states;
    private IStateLogic<T> _defaultState;
    private State _state;

    public StateMachine(State state, T firstState, Dictionary<T, List<StateEnumTransitionPair<T>>> transitions, Dictionary<T, IStateLogic<T>> states, IStateLogic<T> defaultState)
    {
        _transitions = transitions;
        _states = states;
        _defaultState = defaultState;
        _state = state;
        setState(firstState);
        CurrentState.Start(_state);
    }
    public void Execute()
    {
        if (transition(out T? idToTransition))
        {
            setState(idToTransition!.Value);
            CurrentState.Start(_state);
            if (!CurrentState.ExecuteLoopRightAfterTransition)
                return;
        }
        CurrentState.Loop(_state);
    }
    private bool transition(out T? result)
    {
        foreach((T idToTransition, ITransition transition)  in _transitions[CurrentState.ID])
        {
            if (transition.MustTransition(_state))
            {
                result = idToTransition;
                return true;
            }
        }
        result = null;
        return false;
    }
    [MemberNotNull(nameof(CurrentState))]
    private void setState(T state)
    {
        if (!_states.TryGetValue(state, out IStateLogic<T>? newState))
            newState = _defaultState;
        CurrentState = newState;
    }
}
