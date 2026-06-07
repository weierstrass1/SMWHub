using StateMachine;
using System.Text.RegularExpressions;

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
        State.AddVariable(name, new StateVariable<Match>());
        addValidator(new ValidateEntryFormat(this, name));
    }
    public ValidationResult GetFrom(string entry)
    {
        Value = _regex.Match(entry);
        ValidationResult result = validate();

        if (!result.IsValid)
            Value = null;

        return result;
    }
}
