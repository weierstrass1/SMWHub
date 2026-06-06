using FormatReadLibrary.Readers.Validators;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class IntegerIDListStateVariable<TValue> : IStateVariable<int[]>, ISelfValidatedStateVariable
{
    private readonly static Regex _rangeRegex = RegexContainer.RangeRegex();
    public int[]? Value { get; set; }
    public bool CleanOnReset { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (int[]?)value; }
    private readonly Dictionary<int, TValue> _dictionary;
    private readonly int _maxID;
    private readonly bool _allowMultiID;
    public IntegerIDListStateVariable(int maxID = 255, bool allowMultiID = false)
    {
        _dictionary = [];
        _maxID = maxID;
        _allowMultiID = allowMultiID;
    }
    public IntegerIDListStateVariable(Dictionary<int, TValue> dictionary, int maxID = 255, bool allowMultiID = false)
    {
        _dictionary = dictionary;
        _maxID = maxID;
        _allowMultiID = allowMultiID;
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
    private static int[] getSingleValue(Match match)
    {
        return [int.Parse(match.Groups["single"].Value)];
    }
    private static int[] getRangeValue(Match match)
    {
        int start = int.Parse(match.Groups["start"].Value);
        int end = int.Parse(match.Groups["end"].Value);
        return [.. Enumerable.Range(start, end - start + 1)];
    }
    private static int[] getMultiValue(Match match)
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
