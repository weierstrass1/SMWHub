namespace StateMachine.Interfaces;

public interface IStateLogicUpdate<T> : IStateLogic<T> where T : struct, Enum
{
    public void Update(State state);
}
