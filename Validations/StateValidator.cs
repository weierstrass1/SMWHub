using StateMachine;

namespace Validations;

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
    protected virtual ValidationResult getSelfValidatedVariables(string entry)
    {
        ValidationResult result = new();
        var selfValidatedVariables = State.Variables.Values
            .Where(v => v is ISelfValidatedStateVariable)
            .Cast<ISelfValidatedStateVariable>();
        foreach (var variable in selfValidatedVariables)
        {
            result.Merge(variable.GetFrom(entry));
        }
        return result;
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
