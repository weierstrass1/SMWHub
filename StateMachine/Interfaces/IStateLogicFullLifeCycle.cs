namespace StateMachine.Interfaces;

public interface IStateLogicFullLifeCycle<T> : IStateLogicStart<T>, IStateLogicUpdate<T>, IStateLogicExit<T> where T : struct, Enum
{
}
