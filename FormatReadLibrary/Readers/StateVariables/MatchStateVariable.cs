using SMWHubValidations.StateVariableValidations;
using StateMachine;
using StateMachine.Interfaces;
using System.Text.RegularExpressions;
using Validations;
using Validations.Interfaces;

namespace FormatReadLibrary.Readers.StateVariables;

public class MatchStateVariable : StateValidator, IStateVariable<Match>, ISelfValidatedStateVariable
{
    public Match? Value { get => State.Get<Match>(_name); set => State.Set(_name, value); }
    public bool CleanOnReset { get; set; } = false;
    object? IStateVariable.Value { get => Value; set => Value = (Match?)value; }
    private readonly Regex _regex;
    private readonly string _name;
    public MatchStateVariable(string name, Regex regex)
    {
        _regex = regex;
        _name = name;
        State.AddVariable<Match>(name);
        addValidator(new ValidateEntryFormat(this, name));
    }
    public ValidationResult GetFrom(ValidationContext context, string entry)
    {
        Value = _regex.Match(entry);
        Context = context;
        ValidationResult result = validate();

        if (!result.IsValid)
            Value = null;

        return result;
    }
}
