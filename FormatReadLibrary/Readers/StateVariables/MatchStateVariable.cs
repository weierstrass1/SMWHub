using SMWHubValidations.StateVariableValidations;
using System.Text.RegularExpressions;
using Validations;
using Validations.Interfaces;
using ZWXStateMachine.Interfaces;

namespace FormatReadLibrary.Readers.StateVariables;

public class MatchStateVariable : StateValidator, IStateVariable<Match>, ISelfValidatedStateVariable
{
    public Match? Value { get => StateData.Get<Match>(_name); set => StateData.Set(_name, value); }
    public bool CleanOnReset { get; set; } = false;
    object? IStateVariable.Value { get => Value; set => Value = (Match?)value; }
    private readonly Regex _regex;
    private readonly string _name;
    public MatchStateVariable(string name, Regex regex)
    {
        _regex = regex;
        _name = name;
        StateData.AddVariable<Match>(name);
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
