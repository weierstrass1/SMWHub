namespace StateMachine.Interfaces;

public interface IStateLogicStart<T> : IStateLogic<T> where T : struct, Enum
{
    public void Start(State state);
}
