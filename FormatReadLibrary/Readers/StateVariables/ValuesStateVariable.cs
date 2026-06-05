using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class ValuesStateVariable : IStateVariable<int[]>
{
    private readonly static Regex _rangeRegex = FileRegexContainer.RangeRegex();
    public int[]? Value { get; set; }
    public bool CleanOnReset { get; set; }

    object? IStateVariable.Value { get => Value; set => Value = (int[]?)value; }
    public int[]? GetFrom(Match match)
    {
        if (match.Groups["var"].Success)
            return [..match.Groups["var"].Value
                .Split(' ')
                .Select(x => x[0] == '@' ?
                    int.Parse(x[1..]) :
                    Convert.ToInt32(x, 16))];

        if (!match.Groups["ids"].Success)
            return [];

        if (match.Groups["single"].Success)
            return [int.Parse(match.Groups["single"].Value)];
        int start;
        int end;
        if (match.Groups["range"].Success)
        {
            start = int.Parse(match.Groups["start"].Value);
            end = int.Parse(match.Groups["end"].Value);
            return [.. Enumerable.Range(start, end - start + 1)];
        }
        string[] split = match.Groups["multiple"].Value[1..^1].Split(',');
        List<int> values = [];
        Match m;

        foreach (string s in split)
        {
            m = _rangeRegex.Match(s.Trim());
            if (m.Groups["single"].Success)
                values.Add(int.Parse(m.Groups["single"].Value));
            else
            {
                start = int.Parse(m.Groups["start"].Value);
                end = int.Parse(m.Groups["end"].Value);
                values.AddRange(Enumerable.Range(start, end - start + 1));
            }
        }
        return [.. values.Distinct().OrderBy(x => x)];
    }
    public int[]? GetFrom(string line)
    {
        Value = HexUtils.GetValues(line);
        return Value;
    }
}
