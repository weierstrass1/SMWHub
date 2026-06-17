namespace StateMachine.Interfaces;

public interface IStateLogicStartUpdate<T> : IStateLogicStart<T>, IStateLogicUpdate<T> where T : struct, Enum
{
}
