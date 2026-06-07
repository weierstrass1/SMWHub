using StateMachine;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public sealed partial class IntegerIDListStateVariable<TValue> : IStateVariable<int[]>, ISelfValidatedStateVariable
{
    [GeneratedRegex(RegexContainer.RANGE_PATTERN)]
    public static partial Regex RangeRegex();
    [GeneratedRegex(RegexContainer.HEX_RANGE_PATTERN)]
    public static partial Regex HexRangeRegex();
    public int[]? Value { get; set; }
    public bool CleanOnReset { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (int[]?)value; }
    private readonly Dictionary<int, TValue> _dictionary;
    private readonly int _maxID;
    private readonly bool _allowMultiID;
    private readonly Regex _rangeRegex;
    private readonly NumberStyles _style;
    public IntegerIDListStateVariable(int maxID = 255, bool allowMultiID = false, bool useHexIDs = true)
    {
        _dictionary = [];
        _maxID = maxID;
        _allowMultiID = allowMultiID;
        _style = useHexIDs ?
            NumberStyles.HexNumber :
            NumberStyles.Integer;
        _rangeRegex = useHexIDs ?
            HexRangeRegex() :
            RangeRegex();
    }
    public IntegerIDListStateVariable(Dictionary<int, TValue> dictionary, int maxID = 255, bool allowMultiID = false, bool useHexIDs = true)
    {
        _dictionary = dictionary;
        _maxID = maxID;
        _allowMultiID = allowMultiID;
        _style = useHexIDs ?
            NumberStyles.HexNumber :
            NumberStyles.Integer;
        _rangeRegex = useHexIDs ?
            HexRangeRegex() :
            RangeRegex();
    }
    public ValidationResult GetFrom(string text)
    {
        Match match = _rangeRegex.Match(text);
        ValidationResult result;
        if (!match.Success)
        {
            Value = [];
            result = new();
            result.AddError(ValidatorMessagetypeKeys.INVALID_ID);
            return result;
        }

        if (match.Groups["single"].Success)
            Value = getSingleValue(match);
        else if (match.Groups["range"].Success)
            Value = getRangeValue(match);
        else
            Value = getMultiValue(match);

        result = validateValues();
        if (!_allowMultiID && Value.Length != 1)
            result.AddError(ValidatorMessagetypeKeys.MULTI_ID_NOT_ALLOWED);

        if(!result.IsValid)
            Value = [];

        return result;
    }
    private int[] getSingleValue(Match match)
    {
        return [int.Parse(match.Groups["single"].Value, _style)];
    }
    private int[] getRangeValue(Match match)
    {
        int start = int.Parse(match.Groups["start"].Value, _style);
        int end = int.Parse(match.Groups["end"].Value, _style);
        return [.. Enumerable.Range(start, end - start + 1)];
    }
    private int[] getMultiValue(Match match)
    {
        string[] split = [..match.Groups["multiple"].Value[1..^1]
            .Split(',')
            .Select(s => s.Trim())];
        List<int> values = [];
        Match m;

        foreach (string s in split)
        {
            m = _rangeRegex.Match(s.Trim());
            if (m.Groups["single"].Success)
                values.Add(getSingleValue(m)[0]);
            else
                values.AddRange(getRangeValue(m));
        }
        return [.. values.Distinct().OrderBy(x => x)];
    }
    private ValidationResult validateValues()
    {
        ValidationResult result = new();
        IntegerIDStateVariable<TValue> variable = new(_dictionary, _maxID, _allowMultiID);

        foreach(var value in Value!)
        {
            variable.Value = value;
            result.Merge(variable.Validate());
        }
        return result;
    }
}
