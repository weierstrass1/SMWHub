using StateMachine.Interfaces;

namespace StateMachine
{
    public class DelegateStateLogicStart<T>(T id, Action<State> startDelegate) : IStateLogicStart<T> where T : struct, Enum
    {
        public T ID { get; } = id;
        public bool ExecuteLoopRightAfterTransition { get; } = false;
        private readonly Action<State> _startDelegate = startDelegate;
        public void Start(State state)
        {
            _startDelegate(state);
        }
    }
}
