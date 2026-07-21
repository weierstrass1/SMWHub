using Validations.Interfaces;
using ZWXStateMachine;

namespace Validations;

public abstract class StateValidator : IValidationState
{
    public StateData StateData { get; private set; }
    public ValidationContext? Context { get; set; }
    protected List<VariableValidation> _validators { get; private set; }
    public StateValidator()
    {
        StateData = new();
        _validators = [];
    }
    protected void addValidator(VariableValidation validator)
    {
        _validators.Add(validator);
    }
    protected virtual ValidationResult getSelfValidatedVariables(string entry)
    {
        ValidationResult result = new();
        var selfValidatedVariables = StateData.Variables.Values
            .OfType<ISelfValidatedStateVariable>();
        foreach (var variable in selfValidatedVariables)
        {
            result.Merge(variable.GetFrom(Context!, entry));
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
