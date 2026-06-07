using StateMachine;

namespace Validations;

public interface IValidationState : IHaveState
{
    public ValidationContext? Context { get; }
}
