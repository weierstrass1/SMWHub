using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public interface IHaveState
{
    public State State { get; }
}
