using ZWXStateMachine.Interfaces;

namespace Validations.Interfaces;

public interface IValidationState : IHaveStateData
{
    public ValidationContext? Context { get; set; }
}
