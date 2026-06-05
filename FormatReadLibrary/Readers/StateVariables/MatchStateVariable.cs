using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class MatchStateVariable : IStateVariable<Match>
{
    public Match? Value { get; set; }
    public bool CleanOnReset { get; set; } = false;

    object? IStateVariable.Value { get => Value; set => Value = (Match?)value; }
    public Match? GetFrom(string str, Regex regex)
    {
        Value = regex.Match(str);
        return Value;
    }
}
