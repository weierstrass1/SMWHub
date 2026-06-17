using StateMachine.Interfaces;

namespace StateMachine
{
    public class DelegateStateLogicStartUpdate<T>(T id, Action<State> startDelegate, Action<State> updateDelegate, bool executeLoopRightAfterTransition = true) : IStateLogicStartUpdate<T> where T : struct, Enum
    {
        public T ID { get; } = id;
        public bool ExecuteLoopRightAfterTransition { get; set; } = executeLoopRightAfterTransition;
        private readonly Action<State> _startDelegate = startDelegate;
        private readonly Action<State> _updateDelegate = updateDelegate;
        public void Start(State state)
        {
            _startDelegate(state);
        }
        public void Update(State state)
        {
            _updateDelegate(state);
        }
    }
}
