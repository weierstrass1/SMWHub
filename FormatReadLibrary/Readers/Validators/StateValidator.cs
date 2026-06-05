using StateMachine;

namespace FormatReadLibrary.Readers.Validators;

public abstract class StateValidator : IHaveState
{
    public State State { get; private set; }
    protected List<Validator> _validators { get; private set; }
    public StateValidator() 
    {
        State = new();
        _validators = [];
    }
    protected void addValidator(Validator validator)
    {
        _validators.Add(validator);
    }
    protected virtual ValidationResult validate()
    {
        ValidationResult result = new();
        foreach (var validator in _validators)
        {
            result.Merge(validator.Validate(this));
        }
        return result;
    }
}
