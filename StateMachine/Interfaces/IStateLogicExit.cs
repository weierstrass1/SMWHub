namespace StateMachine.Interfaces;

public interface IStateLogicExit<T> : IStateLogic<T> where T : struct, Enum
{
    public void Exit(State state);
}
