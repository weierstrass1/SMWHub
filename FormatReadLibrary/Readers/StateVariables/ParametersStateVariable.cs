using SMWHubValidations;
using StateMachine;
using StateMachine.Interfaces;
using System.Text.RegularExpressions;
using Validations;
using Validations.Interfaces;

namespace FormatReadLibrary.Readers.StateVariables;

public class ParametersStateVariable : StateValidator, IStateVariable<int[]>, ISelfValidatedStateVariable
{
    private readonly static Regex _valuesRegex = RegexContainer.ValuesRegex();
    public int[]? Value
    {
        get => State.Get<int[]>("Parameters");
        set => State.Set("Parameters", value);
    }
    public bool CleanOnReset { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (int[]?)value; }
    public ParametersStateVariable(int minLimit = 0, int maxLimit = 255, bool allowedVariables = false)
    {
        State.AddVariable("Parameters", new StateVariable<int[]>());
        addValidator(new ValidateEntryParameters(this, minLimit, maxLimit, allowedVariables));
    }
    public ValidationResult GetFrom(ValidationContext context, string text)
    {
        Context = context;
        Match match = _valuesRegex.Match(text);
        if (!match.Groups["var"].Success)
        {
            Value = [];
            return new();
        }
        Value = [..match.Groups["var"].Value
                .Split(' ')
                .Select(x => x[0] == '@' ?
                    int.Parse(x[1..]) :
                    Convert.ToInt32(x, 16))];

        ValidationResult result = validate();
        if (!result.IsValid)
            Value = [];

        return result;
    }
}
