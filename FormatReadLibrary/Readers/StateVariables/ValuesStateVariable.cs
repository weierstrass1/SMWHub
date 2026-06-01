using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables
{
    public class ValuesStateVariable : IStateVariable<int[]>
    {
        public int[]? Value { get; set; }
        public bool CleanOnReset { get; set; }

        object? IStateVariable.Value { get => Value; set => Value = (int[]?)value; }
        public int[]? GetFrom(Match match)
        {
            if (!match.Groups["var"].Success)
            {
                Value = [];
                return Value;
            }

            Value = [..match.Groups["var"].Value
                    .Split(' ')
                    .Select(x => x[0] == '@' ?
                        int.Parse(x[1..]) :
                        Convert.ToInt32(x, 16))];
            return Value;
        }
        public int[]? GetFrom(string line)
        {
            Value = HexUtils.GetValues(line);
            return Value;
        }
    }
}
