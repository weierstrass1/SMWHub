using System.Diagnostics.CodeAnalysis;
using ZWXStateMachine.Interfaces;

namespace ZWXStateMachine;

public class StateMachine<T> : Validator, IHaveStateData where T : struct, Enum
{
    public StateData StateData { get; private set; }
    public T CurrentStateID => _currentState.ID;
    public IStateBahaviour CurrentStateLogic => _currentState.StateLogic;
    private readonly Dictionary<T, List<StateIDTransitionPair<T>>> _transitions;
    private readonly Dictionary<T, StateIDBehaviourPair<T>> _states;
    private readonly IStateBahaviour? _defaultState;
    private bool _didntStartYet = true;
    private bool _skipExit = true;
    private StateIDBehaviourPair<T> _currentState;
    public StateMachine(StateData state, T firstState, Dictionary<T, List<StateIDTransitionPair<T>>> transitions, Dictionary<T, StateIDBehaviourPair<T>> states, IStateBahaviour? defaultState = null, params StateIDTransitionPair<T>[] globalTransitions) : base()
    {
        List<StateIDTransitionPair<T>>  gtrans = globalTransitions == null ? [] : [.. globalTransitions
            .Where(gt => gt != null && gt.Transition != default)];

        StateData = state;
        validateAttributes(this);

        _transitions = transitions;
        static int cmp(StateIDTransitionPair<T> t1, StateIDTransitionPair<T> t2)
        {
            if (t1.Priority < t2.Priority)
                return -1;
            if (t1.Priority > t2.Priority)
                return 1;
            return 0;
        }
        foreach (var trans in _transitions)
        {
            trans.Value.AddRange(gtrans.Where(t => !t.IDToTransition.Equals(trans.Key)));
            trans.Value.Sort(cmp);
        }
        _states = states;
        _defaultState = defaultState;

        setState(firstState);
    }
    public void Execute()
    {
        if (transition(out T? idToTransition))
        {
            if (!_skipExit && CurrentStateLogic is IStateBehaviourExit stateWithExit)
                stateWithExit.Exit(StateData);
            _skipExit = false;

            setState(idToTransition!.Value);

            if (CurrentStateLogic is IStateBehaviourEnter stateWithStart)
                stateWithStart.Enter(StateData);

            if (!CurrentStateLogic.ExecuteUpdateRightAfterTransition)
                return;
        }
        if (CurrentStateLogic is IStateBehaviourUpdate stateWithUpdate)
            stateWithUpdate.Update(StateData);
    }
    private bool transition(out T? result)
    {
        if (_didntStartYet)
        {
            result = CurrentStateID;
            _didntStartYet = false;
            return true;
        }
        if (!_transitions.TryGetValue(CurrentStateID, out List<StateIDTransitionPair<T>>? value))
        {
            result = null;
            return false;
        }
        foreach ((T idToTransition, ITransition transition) in value)
        {
            if (!transition.MustTransition(StateData))
                continue;
            result = idToTransition;
            return true;
        }
        result = null;
        return false;
    }
    [MemberNotNull(nameof(_currentState))]
    private void setState(T state)
    {
        if (_defaultState == null)
        {
            _currentState = _states[state];
            return;
        }
        if (!_states.TryGetValue(state, out StateIDBehaviourPair<T>? newState))
            newState = new(state, _defaultState);

        _currentState = newState;
    }
}
