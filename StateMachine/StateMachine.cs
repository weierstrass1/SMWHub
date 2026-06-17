using StateMachine.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace StateMachine;

public class StateMachine<T> : Validator, IHaveState where T : struct, Enum
{
    public State State { get; private set; }
    public IStateLogic<T> CurrentState { get; private set; }
    private readonly Dictionary<T, List<StateEnumTransitionPair<T>>> _transitions;
    private readonly Dictionary<T, IStateLogic<T>> _states;
    private readonly IStateLogic<T>? _defaultState;
    private bool _didntStartYet = false;
    public StateMachine(State state, T firstState, Dictionary<T, List<StateEnumTransitionPair<T>>> transitions, Dictionary<T, IStateLogic<T>> states, IStateLogic<T>? defaultState = null) : base()
    {
        State = state;
        validateAttributes(this);

        _transitions = transitions;
        _states = states;
        _defaultState = defaultState;
        setState(firstState);
    }
    public void Execute()
    {
        if (transition(out T? idToTransition))
        {
            if (CurrentState is IStateLogicExit<T> stateWithExit)
                stateWithExit.Exit(State);
            setState(idToTransition!.Value);
            if (CurrentState is IStateLogicStart<T> stateWithStart)
                stateWithStart.Start(State);
            if (!CurrentState.ExecuteLoopRightAfterTransition)
                return;
        }
        if (CurrentState is IStateLogicUpdate<T> stateWithUpdate)
            stateWithUpdate.Update(State);
    }
    private bool transition(out T? result)
    {
        if(_didntStartYet)
        {
            result = CurrentState.ID;
            _didntStartYet = false;
            return true;
        }
        if(!_transitions.ContainsKey(CurrentState.ID))
        {
            result = null;
            return false;
        }
        foreach((T idToTransition, ITransition transition)  in _transitions[CurrentState.ID])
        {
            if (transition.MustTransition(State))
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
        if(_defaultState == null)
        {
            CurrentState = _states[state];
            return;
        }
        if (!_states.TryGetValue(state, out IStateLogic<T>? newState))
            newState = _defaultState;
        CurrentState = newState;
    }
}
