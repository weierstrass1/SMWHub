using StateMachine;

namespace Validations;

public interface IHaveState
{
    public State State { get; }
}
