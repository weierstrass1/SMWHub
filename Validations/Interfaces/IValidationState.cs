using StateMachine.Interfaces;

namespace Validations.Interfaces;

public interface IValidationState : IHaveState
{
    public ValidationContext? Context { get; }
}
