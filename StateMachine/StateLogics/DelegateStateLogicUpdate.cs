using StateMachine.Interfaces;

namespace StateMachine
{
    public class DelegateStateLogicUpdate<T>(T id, Action<State> updateDelegate, bool executeLoopRightAfterTransition = true) : IStateLogicUpdate<T> where T : struct, Enum
    {
        public T ID { get; } = id;
        public bool ExecuteLoopRightAfterTransition { get; set; } = executeLoopRightAfterTransition;
        private readonly Action<State> _updateDelegate = updateDelegate;
        public void Update(State state)
        {
            _updateDelegate(state);
        }
    }
}
