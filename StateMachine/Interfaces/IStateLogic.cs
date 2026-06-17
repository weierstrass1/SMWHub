namespace StateMachine.Interfaces;

public interface IStateLogic<T> where T : struct, Enum
{
    public T ID { get; }
    public bool ExecuteLoopRightAfterTransition { get; }
    public void Start(State state);
    public void Loop(State state);
}
